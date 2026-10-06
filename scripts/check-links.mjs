// ビルドした dist/ の内部リンクが、すべて存在するページかファイルを指しているかを確かめる。
// 外部リンク（http:、mailto:）とページ内のアンカー（#…）は対象外。
import fs from 'node:fs';
import path from 'node:path';

const root = path.resolve(path.dirname(new URL(import.meta.url).pathname.replace(/^\/([A-Za-z]:)/, '$1')), '..', 'dist');
if (!fs.existsSync(root)) {
	console.error('dist/ not found. Run `npm run build` first.');
	process.exit(1);
}

function* walk(dir) {
	for (const e of fs.readdirSync(dir, { withFileTypes: true })) {
		const p = path.join(dir, e.name);
		if (e.isDirectory()) yield* walk(p);
		else if (e.name.endsWith('.html')) yield p;
	}
}

const broken = [];
let checked = 0;
for (const file of walk(root)) {
	const html = fs.readFileSync(file, 'utf8');
	for (const [, href] of html.matchAll(/href="([^"#?]+)/g)) {
		if (/^(https?:|mailto:|data:)/.test(href) || /\.(css|js|svg|xml)$/.test(href)) continue;
		const target = href.startsWith('/') ? path.join(root, href) : path.resolve(path.dirname(file), href);
		checked++;
		if (!(fs.existsSync(target) && fs.statSync(target).isFile()) && !fs.existsSync(path.join(target, 'index.html')))
			broken.push(`${path.relative(root, file)} -> ${href}`);
	}
}
for (const b of [...new Set(broken)].sort()) console.error(`broken: ${b}`);
console.log(`${checked} internal links checked, ${new Set(broken).size} broken.`);
process.exit(broken.length ? 1 : 0);
