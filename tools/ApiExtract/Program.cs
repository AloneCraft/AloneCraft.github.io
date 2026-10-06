// Omni のソースから、公開 API の一覧を JSON へ書き出す。
// 構文だけを解析する（コンパイルや Unity の DLL は使わない）。どの型を載せるかはサイト側の
// reference/omni/allowlist.yaml が決めるので、ここでは公開型をすべて書き出す。
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiExtract;

internal static class Program {
    // 製品の asmdef が使う define。無効な #if の中の宣言は構文木に入らないため、製品として
    // 有効になる側をすべて定義する。
    private static readonly string[] PreprocessorSymbols = [
        "UNITY_EDITOR","UNITY_6000_0_OR_NEWER","UNITY_2023_1_OR_NEWER","UNITY_2022_1_OR_NEWER",
        "OMNI_INPUT_SYSTEM","OMNI_OBSERVER_HAS_CINEMACHINE","OMNI_OBSERVER_HAS_INPUTSYSTEM",
        "ENABLE_INPUT_SYSTEM",
    ];

    // 載せる候補のフォルダー（Assets/Omni からの相対）。Demo、Verification、テストは対象外。
    private static readonly string[] SourceRoots = ["Runtime","Editor","Converter"];

    private static int Main(string[] args) {
        string? omni = null;
        string? output = null;
        for(int i = 0;i<args.Length;i++) {
            switch(args[i]) {
                case "--omni": omni = args[++i]; break;
                case "--out": output = args[++i]; break;
                default:
                    Console.Error.WriteLine($"unknown argument: {args[i]}");
                    return 2;
            }
        }
        if(omni is null||output is null) {
            Console.Error.WriteLine("usage: ApiExtract --omni <Omni project root> --out <json>");
            return 2;
        }
        string assetRoot = Path.Combine(omni,"Assets","Omni");
        if(!Directory.Exists(assetRoot)) {
            Console.Error.WriteLine($"Assets/Omni not found under {omni}");
            return 1;
        }

        var types = new SortedDictionary<string,TypeInfo>(StringComparer.Ordinal);
        var parseOptions = new CSharpParseOptions(LanguageVersion.Preview,DocumentationMode.Parse,
            SourceCodeKind.Regular,PreprocessorSymbols);
        int fileCount = 0;
        foreach(string root in SourceRoots) {
            string dir = Path.Combine(assetRoot,root);
            if(!Directory.Exists(dir)) continue;
            foreach(string file in Directory.EnumerateFiles(dir,"*.cs",SearchOption.AllDirectories)
                         .OrderBy(p => p,StringComparer.Ordinal)) {
                string relative = Path.GetRelativePath(omni,file).Replace('\\','/');
                if(IsExcludedPath(relative)) continue;
                var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file),parseOptions,file);
                string assembly = FindAssemblyName(file,assetRoot);
                Collect(tree.GetCompilationUnitRoot(),"",null,relative,assembly,types);
                fileCount++;
            }
        }

        var api = new ApiDocument {
            Source = ReadSourceInfo(omni,assetRoot),
            Types = [.. types.Values],
            Hlsl = ExtractHlsl(omni,assetRoot),
        };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
        var json = JsonSerializer.Serialize(api,new JsonSerializerOptions {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });
        File.WriteAllText(output,json.Replace("\r\n","\n")+"\n");
        Console.WriteLine($"{fileCount} files, {types.Count} public types, " +
            $"{api.Hlsl.Functions.Count} HLSL functions -> {output}");
        return 0;
    }

    private static bool IsExcludedPath(string relative) =>
        relative.Contains("/Tests/",StringComparison.Ordinal)||
        relative.Contains("/Test/",StringComparison.Ordinal);

    // ファイルから上へたどって、最初に見つかった asmdef の name を返す。
    private static string FindAssemblyName(string file,string assetRoot) {
        var dir = new DirectoryInfo(Path.GetDirectoryName(file)!);
        string stop = Path.GetFullPath(assetRoot);
        while(dir is not null&&dir.FullName.Length>=stop.Length) {
            var asmdef = dir.GetFiles("*.asmdef").FirstOrDefault();
            if(asmdef is not null) {
                using var doc = JsonDocument.Parse(File.ReadAllText(asmdef.FullName));
                return doc.RootElement.GetProperty("name").GetString()??asmdef.Name;
            }
            dir = dir.Parent;
        }
        return "";
    }

    private static void Collect(SyntaxNode node,string ns,TypeInfo? parent,string file,string assembly,
        SortedDictionary<string,TypeInfo> types) {
        foreach(var child in node.ChildNodes()) {
            switch(child) {
                case BaseNamespaceDeclarationSyntax n:
                    string name = ns.Length==0 ? n.Name.ToString() : ns+"."+n.Name;
                    Collect(n,name,null,file,assembly,types);
                    break;
                case BaseTypeDeclarationSyntax t:
                    CollectType(t,ns,parent,file,assembly,types);
                    break;
                case DelegateDeclarationSyntax d when IsPublic(d.Modifiers,parent):
                    AddDelegate(d,ns,parent,file,assembly,types);
                    break;
            }
        }
    }

    // 入れ子の型は親が公開のときだけ公開になる。interface のメンバーは暗黙に公開。
    private static bool IsPublic(SyntaxTokenList modifiers,TypeInfo? parent) {
        if(parent is not null&&parent.Kind=="interface") return true;
        return modifiers.Any(SyntaxKind.PublicKeyword);
    }

    private static void CollectType(BaseTypeDeclarationSyntax t,string ns,TypeInfo? parent,string file,
        string assembly,SortedDictionary<string,TypeInfo> types) {
        if(!IsPublic(t.Modifiers,parent)) return;
        string kind = t switch {
            ClassDeclarationSyntax => "class",
            StructDeclarationSyntax => "struct",
            InterfaceDeclarationSyntax => "interface",
            EnumDeclarationSyntax => "enum",
            RecordDeclarationSyntax r => r.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword) ? "record struct" : "record",
            _ => "type",
        };
        string typeParams = t is TypeDeclarationSyntax td&&td.TypeParameterList is not null
            ? td.TypeParameterList.ToString() : "";
        string simpleName = t.Identifier.Text+typeParams;
        string fullName = parent is not null ? parent.FullName+"."+simpleName
            : ns.Length==0 ? simpleName : ns+"."+simpleName;

        if(!types.TryGetValue(fullName,out var info)) {
            info = new TypeInfo {
                FullName = fullName,
                Name = simpleName,
                Namespace = ns,
                Kind = kind,
                Assembly = assembly,
                DeclaringType = parent?.FullName,
            };
            types.Add(fullName,info);
        }
        // partial の型は、宣言ごとに属性・基底・メンバーを足し合わせる。
        info.Files.Add(new Location(file,LineOf(t)));
        info.Modifiers = MergeModifiers(info.Modifiers,t.Modifiers);
        info.Attributes.AddRange(ReadAttributes(t.AttributeLists));
        if(t.BaseList is not null&&info.BaseList is null)
            info.BaseList = string.Join(", ",t.BaseList.Types.Select(b => b.ToString()));
        info.Summary ??= ReadSummary(t);
        info.Signature ??= TypeSignature(t,info,typeParams);

        if(t is EnumDeclarationSyntax e) {
            foreach(var m in e.Members) {
                info.Members.Add(new MemberInfo {
                    Kind = "enumMember",
                    Name = m.Identifier.Text,
                    Signature = m.Identifier.Text+(m.EqualsValue is null ? "" : " "+m.EqualsValue.ToString()),
                    DefaultValue = m.EqualsValue?.Value.ToString(),
                    Attributes = ReadAttributes(m.AttributeLists),
                    Summary = ReadSummary(m),
                    Line = LineOf(m),
                    File = file,
                });
            }
            return;
        }
        if(t is not TypeDeclarationSyntax type) return;
        foreach(var member in type.Members) {
            if(member is BaseTypeDeclarationSyntax nested) {
                CollectType(nested,ns,info,file,assembly,types);
                continue;
            }
            if(member is DelegateDeclarationSyntax del) {
                if(IsPublic(del.Modifiers,info)) AddDelegate(del,ns,info,file,assembly,types);
                continue;
            }
            foreach(var m in ReadMember(member,info,file)) info.Members.Add(m);
        }
    }

    private static void AddDelegate(DelegateDeclarationSyntax d,string ns,TypeInfo? parent,string file,
        string assembly,SortedDictionary<string,TypeInfo> types) {
        string name = d.Identifier.Text+(d.TypeParameterList?.ToString()??"");
        string fullName = parent is not null ? parent.FullName+"."+name : ns.Length==0 ? name : ns+"."+name;
        var stripped = d.WithAttributeLists(default).WithoutTrivia();
        types[fullName] = new TypeInfo {
            FullName = fullName,
            Name = name,
            Namespace = ns,
            Kind = "delegate",
            Assembly = assembly,
            DeclaringType = parent?.FullName,
            Signature = Normalize(stripped),
            Summary = ReadSummary(d),
            Attributes = ReadAttributes(d.AttributeLists),
            Files = [new Location(file,LineOf(d))],
        };
    }

    private static IEnumerable<MemberInfo> ReadMember(MemberDeclarationSyntax member,TypeInfo owner,string file) {
        bool ownerIsInterface = owner.Kind=="interface";
        var attributes = ReadAttributes(member.AttributeLists);
        bool isPublic = ownerIsInterface||member.Modifiers.Any(SyntaxKind.PublicKeyword);
        string? summary = ReadSummary(member);
        int line = LineOf(member);

        switch(member) {
            case FieldDeclarationSyntax f: {
                bool isConst = f.Modifiers.Any(SyntaxKind.ConstKeyword);
                bool isStatic = f.Modifiers.Any(SyntaxKind.StaticKeyword)||isConst;
                bool isReadonly = f.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
                bool serializeField = attributes.Any(a => a.Name=="SerializeField"||a.Name=="SerializeReference");
                bool nonSerialized = attributes.Any(a => a.Name=="NonSerialized");
                // Unity が保存する（Inspector に出せる）フィールド: 公開のインスタンスフィールドか、
                // [SerializeField] を付けたもの。型が保存可能かは構文だけでは判定しない。
                bool serialized = !isStatic&&!isReadonly&&!nonSerialized&&(isPublic||serializeField);
                if(!isPublic&&!serialized) yield break;
                foreach(var v in f.Declaration.Variables) {
                    var mods = string.Join(" ",f.Modifiers.Select(m => m.Text));
                    yield return new MemberInfo {
                        Kind = isConst ? "const" : "field",
                        Name = v.Identifier.Text,
                        Signature = $"{mods} {f.Declaration.Type} {v.Identifier.Text}".Trim()+
                            (isConst&&v.Initializer is not null ? " "+v.Initializer : ""),
                        Type = f.Declaration.Type.ToString(),
                        DefaultValue = v.Initializer?.Value.ToString(),
                        IsStatic = isStatic,
                        IsPublic = isPublic,
                        Serialized = serialized,
                        Attributes = attributes,
                        Summary = summary,
                        Line = line,
                        File = file,
                    };
                }
                yield break;
            }
            case EventFieldDeclarationSyntax ef when isPublic:
                foreach(var v in ef.Declaration.Variables) {
                    var mods = string.Join(" ",ef.Modifiers.Select(m => m.Text));
                    yield return new MemberInfo {
                        Kind = "event",
                        Name = v.Identifier.Text,
                        Signature = $"{mods} event {ef.Declaration.Type} {v.Identifier.Text}".Trim(),
                        Type = ef.Declaration.Type.ToString(),
                        IsStatic = ef.Modifiers.Any(SyntaxKind.StaticKeyword),
                        IsPublic = true,
                        Attributes = attributes,
                        Summary = summary,
                        Line = line,
                        File = file,
                    };
                }
                yield break;
            case PropertyDeclarationSyntax p when isPublic:
                yield return new MemberInfo {
                    Kind = "property",
                    Name = p.Identifier.Text,
                    Signature = PropertySignature(p),
                    Type = p.Type.ToString(),
                    DefaultValue = p.Initializer?.Value.ToString(),
                    IsStatic = p.Modifiers.Any(SyntaxKind.StaticKeyword),
                    IsPublic = true,
                    Attributes = attributes,
                    Summary = summary,
                    Line = line,
                    File = file,
                };
                yield break;
            case IndexerDeclarationSyntax ix when isPublic:
                yield return new MemberInfo {
                    Kind = "indexer",
                    Name = "this[]",
                    Signature = IndexerSignature(ix),
                    Type = ix.Type.ToString(),
                    Parameters = ParameterTypes(ix.ParameterList.Parameters),
                    IsPublic = true,
                    Attributes = attributes,
                    Summary = summary,
                    Line = line,
                    File = file,
                };
                yield break;
            case MethodDeclarationSyntax m when isPublic:
                yield return new MemberInfo {
                    Kind = "method",
                    Name = m.Identifier.Text+(m.TypeParameterList?.ToString()??""),
                    Signature = Normalize(m.WithBody(null).WithExpressionBody(null)
                        .WithSemicolonToken(default).WithAttributeLists(default).WithoutTrivia()),
                    Type = m.ReturnType.ToString(),
                    Parameters = ParameterTypes(m.ParameterList.Parameters),
                    IsStatic = m.Modifiers.Any(SyntaxKind.StaticKeyword),
                    IsPublic = true,
                    Attributes = attributes,
                    Summary = summary,
                    Line = line,
                    File = file,
                };
                yield break;
            case ConstructorDeclarationSyntax c when isPublic:
                yield return new MemberInfo {
                    Kind = "constructor",
                    Name = owner.Name,
                    Signature = Normalize(c.WithBody(null).WithExpressionBody(null).WithInitializer(null)
                        .WithSemicolonToken(default).WithAttributeLists(default).WithoutTrivia()),
                    Parameters = ParameterTypes(c.ParameterList.Parameters),
                    IsStatic = c.Modifiers.Any(SyntaxKind.StaticKeyword),
                    IsPublic = true,
                    Attributes = attributes,
                    Summary = summary,
                    Line = line,
                    File = file,
                };
                yield break;
            case OperatorDeclarationSyntax o when isPublic:
                yield return new MemberInfo {
                    Kind = "operator",
                    Name = "operator "+o.OperatorToken.Text,
                    Signature = Normalize(o.WithBody(null).WithExpressionBody(null)
                        .WithSemicolonToken(default).WithAttributeLists(default).WithoutTrivia()),
                    Type = o.ReturnType.ToString(),
                    Parameters = ParameterTypes(o.ParameterList.Parameters),
                    IsStatic = true,
                    IsPublic = true,
                    Attributes = attributes,
                    Summary = summary,
                    Line = line,
                    File = file,
                };
                yield break;
            case ConversionOperatorDeclarationSyntax co when isPublic:
                yield return new MemberInfo {
                    Kind = "conversion",
                    Name = $"{co.ImplicitOrExplicitKeyword.Text} operator {co.Type}",
                    Signature = Normalize(co.WithBody(null).WithExpressionBody(null)
                        .WithSemicolonToken(default).WithAttributeLists(default).WithoutTrivia()),
                    Type = co.Type.ToString(),
                    Parameters = ParameterTypes(co.ParameterList.Parameters),
                    IsStatic = true,
                    IsPublic = true,
                    Attributes = attributes,
                    Summary = summary,
                    Line = line,
                    File = file,
                };
                yield break;
        }
    }

    private static List<string> ParameterTypes(SeparatedSyntaxList<ParameterSyntax> parameters) =>
        [.. parameters.Select(p => string.Join(" ",p.Modifiers.Select(m => m.Text).Append(p.Type?.ToString()??""))
            .Trim())];

    // 本体を取り除き、アクセサーだけを「{ get; set; }」の形で残す。
    private static string PropertySignature(PropertyDeclarationSyntax p) {
        string accessors = p.AccessorList is null
            ? "get;"
            : string.Join(" ",p.AccessorList.Accessors.Select(a =>
                (string.Join(" ",a.Modifiers.Select(m => m.Text))+" "+a.Keyword.Text+";").Trim()));
        string mods = string.Join(" ",p.Modifiers.Select(m => m.Text));
        return $"{mods} {p.Type} {p.Identifier.Text} {{ {accessors} }}".Trim();
    }

    private static string IndexerSignature(IndexerDeclarationSyntax ix) {
        string accessors = ix.AccessorList is null
            ? "get;"
            : string.Join(" ",ix.AccessorList.Accessors.Select(a =>
                (string.Join(" ",a.Modifiers.Select(m => m.Text))+" "+a.Keyword.Text+";").Trim()));
        string mods = string.Join(" ",ix.Modifiers.Select(m => m.Text));
        return $"{mods} {ix.Type} this{ix.ParameterList} {{ {accessors} }}".Trim();
    }

    private static string TypeSignature(BaseTypeDeclarationSyntax t,TypeInfo info,string typeParams) {
        string mods = string.Join(" ",t.Modifiers.Where(m => !m.IsKind(SyntaxKind.PartialKeyword)).Select(m => m.Text));
        string keyword = info.Kind=="record struct" ? "record struct" : info.Kind;
        string bases = t.BaseList is null ? "" : " : "+string.Join(", ",t.BaseList.Types.Select(b => b.ToString()));
        return $"{mods} {keyword} {t.Identifier.Text}{typeParams}{bases}".Trim();
    }

    private static string MergeModifiers(string? current,SyntaxTokenList modifiers) {
        var set = new List<string>((current??"").Split(' ',StringSplitOptions.RemoveEmptyEntries));
        foreach(var m in modifiers) if(!set.Contains(m.Text)) set.Add(m.Text);
        return string.Join(" ",set);
    }

    private static string Normalize(SyntaxNode node) =>
        Regex.Replace(node.NormalizeWhitespace(" ","\n").ToFullString().Replace("\n"," "),@"\s+"," ").Trim();

    private static int LineOf(SyntaxNode node) =>
        node.GetLocation().GetLineSpan().StartLinePosition.Line+1;

    private static List<AttributeInfo> ReadAttributes(SyntaxList<AttributeListSyntax> lists) {
        var result = new List<AttributeInfo>();
        foreach(var list in lists)
        foreach(var a in list.Attributes) {
            string name = a.Name.ToString();
            int dot = name.LastIndexOf('.');
            if(dot>=0) name = name[(dot+1)..];
            if(name.EndsWith("Attribute",StringComparison.Ordinal)) name = name[..^"Attribute".Length];
            result.Add(new AttributeInfo(name,a.ArgumentList is null
                ? null
                : string.Join(", ",a.ArgumentList.Arguments.Select(x => x.ToString()))));
        }
        return result;
    }

    // XML ドキュメントコメントの <summary> を平文にする（<see cref="X"/> は X、<c>x</c> は x）。
    private static string? ReadSummary(SyntaxNode node) {
        var doc = node.GetLeadingTrivia()
            .Select(t => t.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .LastOrDefault();
        if(doc is null) return null;
        string raw = string.Join("\n",doc.ToFullString().Split('\n')
            .Select(l => Regex.Replace(l,@"^\s*///\s?","")));
        try {
            var root = XElement.Parse("<doc>"+raw+"</doc>",LoadOptions.PreserveWhitespace);
            var summary = root.Element("summary");
            if(summary is null) return null;
            string text = string.Concat(summary.Nodes().Select(FlattenXml));
            text = Regex.Replace(text,@"[ \t]*\n[ \t]*","\n").Trim();
            return text.Length==0 ? null : text;
        }
        catch(System.Xml.XmlException) {
            var m = Regex.Match(raw,@"<summary>(.*?)</summary>",RegexOptions.Singleline);
            return m.Success ? m.Groups[1].Value.Trim() : null;
        }
    }

    private static string FlattenXml(XNode node) => node switch {
        XText text => text.Value,
        XElement e when e.Name.LocalName is "see" or "seealso" =>
            (string?)e.Attribute("cref") is { } cref ? StripCref(cref)
            : (string?)e.Attribute("langword")??(string?)e.Attribute("href")??e.Value,
        XElement e when e.Name.LocalName is "paramref" or "typeparamref" => (string?)e.Attribute("name")??"",
        XElement e => string.Concat(e.Nodes().Select(FlattenXml)),
        _ => "",
    };

    private static string StripCref(string cref) {
        int colon = cref.IndexOf(':');
        return colon==1 ? cref[2..] : cref;
    }

    private static SourceInfo ReadSourceInfo(string omni,string assetRoot) {
        string? commit = RunGit(omni,"rev-parse","HEAD");
        string? status = RunGit(omni,"status","--porcelain","--","Assets/Omni");
        string catalog = Path.Combine(assetRoot,"Runtime","Render","OmniShaderCatalog.cs");
        string? version = null;
        if(File.Exists(catalog)) {
            var m = Regex.Match(File.ReadAllText(catalog),@"m_ProductVersion\s*=\s*""([^""]+)""");
            if(m.Success) version = m.Groups[1].Value;
        }
        return new SourceInfo(commit?.Trim(),!string.IsNullOrWhiteSpace(status),version);
    }

    private static string? RunGit(string dir,params string[] args) {
        var psi = new ProcessStartInfo("git") {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        psi.ArgumentList.Add("-C");
        psi.ArgumentList.Add(dir);
        foreach(var a in args) psi.ArgumentList.Add(a);
        try {
            using var p = Process.Start(psi)!;
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return p.ExitCode==0 ? output : null;
        }
        catch(System.ComponentModel.Win32Exception) {
            return null;
        }
    }

    private static bool IsShaderDefinition(string line,string name) =>
        Regex.IsMatch(line,$@"^\s*#\s*define\s+{Regex.Escape(name)}\b")||
        Regex.IsMatch(line,$@"^\s*(?:uniform\s+)?(?:float|half|int|uint|bool)[1-4]?(?:x[1-4])?\s+{Regex.Escape(name)}\s*[;\[]");

    private static readonly Regex HlslFunction = new(
        @"^[ \t]*(?:inline[ \t]+)?(?<ret>void|bool|int|uint|float[1-4]?(?:x[1-4])?|half[1-4]?(?:x[1-4])?|real[1-4]?)[ \t]+(?<name>[A-Za-z_]\w*)[ \t]*\((?<params>[^)]*)\)",
        RegexOptions.Multiline|RegexOptions.Compiled);

    private static readonly Regex ShaderIdentifier = new(
        @"\b(?:_OmniRadialCamera_\w+|_RadialGlobal_\w+|OMNI_RADIAL_\w+|OMNI_BRG_\w+|OMNI_VFX_\w+)\b",
        RegexOptions.Compiled);

    // HLSL の関数の宣言と、Shader 向けの識別子（グローバルのプロパティと #define）を集める。
    private static HlslInfo ExtractHlsl(string omni,string assetRoot) {
        var info = new HlslInfo();
        string includeDir = Path.Combine(assetRoot,"Runtime","Sharders");
        if(Directory.Exists(includeDir)) {
            foreach(string file in Directory.EnumerateFiles(includeDir,"*.hlsl").OrderBy(p => p,StringComparer.Ordinal)) {
                string relative = Path.GetRelativePath(omni,file).Replace('\\','/');
                string text = File.ReadAllText(file);
                info.Includes.Add(relative);
                foreach(Match m in HlslFunction.Matches(text)) {
                    int line = text[..m.Index].Count(c => c=='\n')+1;
                    string parameters = Regex.Replace(m.Groups["params"].Value,@"\s+"," ").Trim();
                    info.Functions.Add(new HlslFunction(relative,m.Groups["name"].Value,m.Groups["ret"].Value,
                        parameters,line));
                }
            }
        }
        // 識別子の場所は、HLSL の定義（#define や変数の宣言）を優先し、無ければ最初に現れた所にする。
        var seen = new SortedDictionary<string,Location>(StringComparer.Ordinal);
        var fallback = new SortedDictionary<string,Location>(StringComparer.Ordinal);
        foreach(string sub in new[] { "Runtime","Radial" }) {
            string dir = Path.Combine(assetRoot,sub);
            if(!Directory.Exists(dir)) continue;
            foreach(string file in Directory.EnumerateFiles(dir,"*.*",SearchOption.AllDirectories)
                         .Where(p => p.EndsWith(".hlsl")||p.EndsWith(".cs")||p.EndsWith(".shader"))
                         .OrderBy(p => p.EndsWith(".hlsl") ? 0 : p.EndsWith(".shader") ? 1 : 2)
                         .ThenBy(p => p,StringComparer.Ordinal)) {
                bool shaderSource = !file.EndsWith(".cs");
                string relative = Path.GetRelativePath(omni,file).Replace('\\','/');
                string[] lines = File.ReadAllLines(file);
                for(int i = 0;i<lines.Length;i++)
                foreach(Match m in ShaderIdentifier.Matches(lines[i])) {
                    var location = new Location(relative,i+1);
                    if(shaderSource&&IsShaderDefinition(lines[i],m.Value)) seen.TryAdd(m.Value,location);
                    fallback.TryAdd(m.Value,location);
                }
            }
        }
        foreach(var (name,location) in fallback) seen.TryAdd(name,location);
        info.Identifiers = seen;
        return info;
    }
}

internal sealed record SourceInfo(string? Commit,bool Dirty,string? ProductVersion);

internal sealed record Location(string File,int Line);

internal sealed record AttributeInfo(string Name,string? Arguments);

internal sealed record HlslFunction(string File,string Name,string ReturnType,string Parameters,int Line);

internal sealed class HlslInfo {
    public List<string> Includes { get; set; } = [];
    public List<HlslFunction> Functions { get; set; } = [];
    public SortedDictionary<string,Location> Identifiers { get; set; } = new(StringComparer.Ordinal);
}

internal sealed class ApiDocument {
    public required SourceInfo Source { get; init; }
    public required List<TypeInfo> Types { get; init; }
    public required HlslInfo Hlsl { get; init; }
}

internal sealed class TypeInfo {
    public required string FullName { get; init; }
    public required string Name { get; init; }
    public required string Namespace { get; init; }
    public required string Kind { get; init; }
    public required string Assembly { get; init; }
    public string? DeclaringType { get; init; }
    public string? Modifiers { get; set; }
    public string? Signature { get; set; }
    public string? BaseList { get; set; }
    public string? Summary { get; set; }
    public List<AttributeInfo> Attributes { get; set; } = [];
    public List<Location> Files { get; set; } = [];
    public List<MemberInfo> Members { get; set; } = [];
}

internal sealed class MemberInfo {
    public required string Kind { get; init; }
    public required string Name { get; init; }
    public required string Signature { get; init; }
    public string? Type { get; init; }
    public List<string>? Parameters { get; init; }
    public string? DefaultValue { get; init; }
    public bool IsStatic { get; init; }
    public bool IsPublic { get; init; }
    public bool Serialized { get; init; }
    public List<AttributeInfo> Attributes { get; init; } = [];
    public string? Summary { get; init; }
    public required string File { get; init; }
    public int Line { get; init; }
}
