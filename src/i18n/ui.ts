// ブランドページ（Starlight 外の独自ページ）の文言。日本語が正本、英語はその翻訳。
// Starlight のドキュメント側の文言は src/content/docs/<lang>/ に置く。

export const languages = { ja: '日本語', en: 'English' } as const;
export type Lang = keyof typeof languages;
export const langs = Object.keys(languages) as Lang[];

export function otherLang(lang: Lang): Lang {
	return lang === 'ja' ? 'en' : 'ja';
}

export const ui = {
	ja: {
		'nav.products': '製品',
		'nav.docs': 'ドキュメント',
		'nav.lang': 'English',
		'footer.note': '© AloneCraft',

		'home.title': 'AloneCraft',
		'home.lead': 'AloneCraft の製品とドキュメント。',
		'home.products': '製品',
		'home.omni.tagline': '陽子から観測可能な宇宙まで、ひとつの空間で。',
		'home.omni.body':
			'位置を 192 bit の Logical 座標で持ち、対数の写像で 1 台のカメラに描く Unity アセット。',
		'home.omni.status': '販売準備中 · 0.1.0',
		'home.omni.more': '詳しく見る',

		'omni.title': 'Omni',
		'omni.description':
			'巨大な座標と微小な細部を、精度を落とさずに Unity で描くためのアセット。',
		'omni.hero.kicker': 'Unity アセット · URP',
		'omni.hero.headline': '陽子から、観測可能な宇宙まで。',
		'omni.hero.lead':
			'Omni は位置を 192 bit の Logical 座標で持ち、Unity の float へは表示の直前でだけ変換します。対数の写像（Radial）によって、10⁻¹⁵ m の細部から 10²⁷ m の距離までを、1 台のカメラで途切れずに描きます。',
		'omni.hero.docs': 'ドキュメント',
		'omni.hero.start': 'はじめる',
		'omni.status': '販売準備中。公開日と価格は決まりしだい掲載します。',

		'omni.numbers.title': '数字で見る Omni',
		'omni.numbers.range': '座標の範囲',
		'omni.numbers.rangeNote': '観測可能な宇宙の直径（約 8.8×10²⁶ m）の約 90 倍',
		'omni.numbers.resolution': '座標の分解能',
		'omni.numbers.resolutionNote': 'Q95.96 の 192 bit 固定小数点',
		'omni.numbers.camera': 'カメラ',
		'omni.numbers.cameraValue': '1 台',
		'omni.numbers.cameraNote': '近景から遠景までを分けずに描く',

		'omni.features.title': 'できること',
		'omni.f1.title': 'Logical 座標が正本',
		'omni.f1.body':
			'位置・回転・Scale の正本は Omni.Logical.Transform です。Unity の Transform は有限な値へ変換した表示用の写し（Proxy）で、遠い位置の正本を float に丸めません。溢れる演算はすべて Try で始まる API で、結果を黙って丸めたり飽和させたりしません。',
		'omni.f2.title': '対数の写像で描く',
		'omni.f2.body':
			'目からの距離 d を F(d) = log₂(1 + d·M) へ写して描きます。写像は視線の向きを変えないので、近くの画像は標準の描画と一致し、遠くは圧縮されて同じ画面に収まります。調整するのは View Scale M だけです。',
		'omni.f3.title': 'すべての描画経路で同じ空間',
		'omni.f3.body':
			'色だけでなく、深度（SV_Depth）、影、Culling、Picking、Motion Vectors も同じ空間の約束を守ります。Scene View、Game View、Player の間で、ある経路だけが正しく見える状態を許しません。',
		'omni.f4.title': 'URP の Shader をそのまま',
		'omni.f4.body':
			'URP の標準 Shader は Radial 版を事前に作って同梱しています。独自の Shader、Shader Graph、VFX Graph はプロジェクトの中で変換し、生成物はパッケージと別の Assets/Radial に置くので、更新で上書きされません。',
		'omni.f5.title': '照明を壊さない',
		'omni.f5.body':
			'照明は写像後の位置ではなく物理的な位置で計算します。Light Probe、APV、Lightmap、Reflection Probe は、焼いた空間と表示の空間を対応付ける Component で扱います。正しく照らせない経路は、もっともらしい絵を出さずに止めます（Fail Closed）。',
		'omni.f6.title': 'Editor で確かめられる',
		'omni.f6.body':
			'Scene View の Overlay から、View ごとに標準と Radial を切り替えられます。導入したプロジェクトでは、Verification Hub で深度・影・VFX・カメラなどの描画を Case ごとに確かめ、結果を JSON で残せます。',

		'omni.demos.title': 'デモ',
		'omni.demos.lead': '製品にはデモの Scene が入っています。画像は CosmicHierarchy の録画から切り出したものです。',
		'omni.demo.scale.title': 'CosmicScale',
		'omni.demo.scale.body':
			'陽子から観測可能な宇宙まで、カメラが後ろへ下がり続けて大きさを比べます。木星から先の天体は画像を使わず、Shader で手続き生成しています。',
		'omni.demo.hierarchy.title': 'CosmicHierarchy',
		'omni.demo.hierarchy.body':
			'月・地球・太陽・天の川銀河・超銀河団の入れ子を、公表された位置に置いた 3D 空間で見せます。最後はいて座A* へ落ちて事象の地平面を越えます。',
		'omni.demo.solar.title': 'SolarSystem',
		'omni.demo.solar.body':
			'太陽、8 惑星、月と 19 の衛星の計 29 天体を、実寸・実距離・実時刻（1900〜2050 年）で見て回ります。',
		'omni.demo.earth.title': 'EarthToMoon',
		'omni.demo.earth.body': '巨大な座標、天体の間の移動、月面の詳細を一つにまとめたデモです。',
		'omni.demo.caption.fall': 'いて座A* へ落ちるカメラから見た、曲げられた円盤の光（CosmicHierarchy）',
		'omni.demo.caption.mw': '天の川銀河の円盤の外へ（CosmicHierarchy）',

		'omni.req.title': '動作環境（0.1）',
		'omni.req.unity': 'Unity 6000.6.0f1',
		'omni.req.urp': 'URP 17.6.0（Forward、Forward+）',
		'omni.req.input': 'Input System 1.20.0',
		'omni.req.targets': '受入済みのビルド対象: Windows（Direct3D 12）、Web（WebGPU）',
		'omni.req.more': '詳しい対応表',
	},
	en: {
		'nav.products': 'Products',
		'nav.docs': 'Docs',
		'nav.lang': '日本語',
		'footer.note': '© AloneCraft',

		'home.title': 'AloneCraft',
		'home.lead': 'Products and documentation from AloneCraft.',
		'home.products': 'Products',
		'home.omni.tagline': 'From a proton to the observable universe, in one space.',
		'home.omni.body':
			'A Unity asset that keeps positions in 192-bit Logical coordinates and draws them with one camera through a logarithmic mapping.',
		'home.omni.status': 'Coming soon · 0.1.0',
		'home.omni.more': 'Learn more',

		'omni.title': 'Omni',
		'omni.description':
			'A Unity asset for rendering huge coordinates and tiny details without losing precision.',
		'omni.hero.kicker': 'Unity asset · URP',
		'omni.hero.headline': 'From a proton to the observable universe.',
		'omni.hero.lead':
			'Omni keeps positions in 192-bit Logical coordinates and converts them to Unity floats only at the moment of display. Its logarithmic mapping (Radial) lets a single camera draw everything from 10⁻¹⁵ m details to 10²⁷ m distances without seams.',
		'omni.hero.docs': 'Documentation',
		'omni.hero.start': 'Get started',
		'omni.status': 'Coming soon. Release date and price will be announced here.',

		'omni.numbers.title': 'Omni in numbers',
		'omni.numbers.range': 'Coordinate range',
		'omni.numbers.rangeNote': 'About 90 times the diameter of the observable universe (about 8.8×10²⁶ m)',
		'omni.numbers.resolution': 'Coordinate resolution',
		'omni.numbers.resolutionNote': '192-bit Q95.96 fixed point',
		'omni.numbers.camera': 'Cameras',
		'omni.numbers.cameraValue': '1',
		'omni.numbers.cameraNote': 'No split between near and far rendering',

		'omni.features.title': 'What it does',
		'omni.f1.title': 'Logical coordinates are the source of truth',
		'omni.f1.body':
			'Position, rotation and scale live in Omni.Logical.Transform. The Unity Transform is a finite display copy (a proxy), so the true value of a far position is never rounded to float. Every operation that can overflow is a Try API; results are never silently rounded or clamped.',
		'omni.f2.title': 'Drawn through a logarithmic mapping',
		'omni.f2.body':
			'The distance d from the eye is mapped to F(d) = log₂(1 + d·M). The mapping keeps view directions, so nearby pixels match standard rendering while far objects are compressed into the same frame. The only setting is the View Scale M.',
		'omni.f3.title': 'One space for every render path',
		'omni.f3.body':
			'Not only color but depth (SV_Depth), shadows, culling, picking and motion vectors follow the same spatial contract. Omni does not accept a state where only one of Scene View, Game View or Player looks right.',
		'omni.f4.title': 'Your URP shaders, as they are',
		'omni.f4.body':
			'Radial versions of the standard URP shaders are prebuilt and included. Your own shaders, Shader Graphs and VFX Graphs are converted inside your project, and the output goes to Assets/Radial, separate from the package, so updates never overwrite it.',
		'omni.f5.title': 'Lighting stays correct',
		'omni.f5.body':
			'Lighting uses the physical position, not the mapped one. Light Probes, APV, Lightmaps and Reflection Probes are handled by components that link the baked space to the displayed space. A path that cannot be lit correctly stops instead of showing a plausible image (fail closed).',
		'omni.f6.title': 'Checkable in the Editor',
		'omni.f6.body':
			'The Scene View overlay switches each view between standard and Radial. In your project, the Verification Hub checks depth, shadows, VFX, cameras and more case by case, and saves the results as JSON.',

		'omni.demos.title': 'Demos',
		'omni.demos.lead': 'The package includes demo scenes. The images are cropped from a CosmicHierarchy recording.',
		'omni.demo.scale.title': 'CosmicScale',
		'omni.demo.scale.body':
			'The camera keeps moving back, from a proton to the observable universe, comparing sizes. Bodies from Jupiter onward use no textures; they are generated procedurally in shaders.',
		'omni.demo.hierarchy.title': 'CosmicHierarchy',
		'omni.demo.hierarchy.body':
			'Shows the nesting of the Moon, Earth, Sun, Milky Way and superclusters in a 3D space built from published positions. It ends by falling into Sagittarius A* and crossing the event horizon.',
		'omni.demo.solar.title': 'SolarSystem',
		'omni.demo.solar.body':
			'Tour 29 bodies (the Sun, 8 planets, the Moon and 19 other moons) at real size, real distance and real time (1900–2050).',
		'omni.demo.earth.title': 'EarthToMoon',
		'omni.demo.earth.body': 'Combines huge coordinates, travel between bodies and lunar surface detail in one demo.',
		'omni.demo.caption.fall': 'The bent light of the disk, seen by a camera falling into Sagittarius A* (CosmicHierarchy)',
		'omni.demo.caption.mw': 'Leaving the disk of the Milky Way (CosmicHierarchy)',

		'omni.req.title': 'Requirements (0.1)',
		'omni.req.unity': 'Unity 6000.6.0f1',
		'omni.req.urp': 'URP 17.6.0 (Forward, Forward+)',
		'omni.req.input': 'Input System 1.20.0',
		'omni.req.targets': 'Accepted build targets: Windows (Direct3D 12), Web (WebGPU)',
		'omni.req.more': 'Full support table',
	},
} as const;

export type UiKey = keyof (typeof ui)['ja'];

export function t(lang: Lang, key: UiKey): string {
	return ui[lang][key];
}
