// Omni のソースから src/data/api/omni.json を作る（tools/ApiExtract を実行する）。
// Omni の場所は環境変数 OMNI_ROOT で変えられる。既定は開発機の D:/Unity/6.6/Omni。
import { spawnSync } from 'node:child_process';
import path from 'node:path';

const root = path.resolve(path.dirname(new URL(import.meta.url).pathname.replace(/^\/([A-Za-z]:)/, '$1')), '..');
const omni = process.env.OMNI_ROOT ?? 'D:/Unity/6.6/Omni';
const result = spawnSync(
	'dotnet',
	['run', '--project', path.join(root, 'tools/ApiExtract'), '--', '--omni', omni, '--out', path.join(root, 'src/data/api/omni.json')],
	{ stdio: 'inherit' },
);
process.exit(result.status ?? 1);
