// @ts-check
import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

// リファレンスのカテゴリ（reference/omni/allowlist.yaml の categories と同じ ID）。
function referenceGroups(categories) {
	return categories.map(([id, ja, en]) => ({
		label: ja,
		translations: { en },
		collapsed: true,
		items: [{ autogenerate: { directory: `omni/reference/${id}` } }],
	}));
}

// 公開先は GitHub Pages の AloneCraft.github.io（組織のサイトなので base は不要）。独自ドメインにしたら site を変える。
export default defineConfig({
	site: 'https://alonecraft.github.io',
	integrations: [
		starlight({
			title: 'AloneCraft',
			defaultLocale: 'ja',
			locales: {
				ja: { label: '日本語', lang: 'ja' },
				en: { label: 'English', lang: 'en' },
			},
			customCss: ['./src/styles/starlight.css'],
			sidebar: [
				{
					label: 'Omni',
					items: [
						{ label: '概要', translations: { en: 'Overview' }, slug: 'omni/docs' },
						{
							label: 'はじめに',
							translations: { en: 'Getting started' },
							items: [
								{ slug: 'omni/docs/getting-started/requirements' },
								{ slug: 'omni/docs/getting-started/installation' },
								{ slug: 'omni/docs/getting-started/verification' },
								{ slug: 'omni/docs/getting-started/switch-rendering' },
							],
						},
						{
							label: '概念',
							translations: { en: 'Concepts' },
							items: [{ autogenerate: { directory: 'omni/docs/concepts' } }],
						},
						{
							label: 'ガイド',
							translations: { en: 'Guides' },
							items: [{ autogenerate: { directory: 'omni/docs/guides' } }],
						},
					],
				},
				{
					label: 'Omni リファレンス',
					translations: { en: 'Omni reference' },
					items: [
						{ label: '概要', translations: { en: 'Overview' }, slug: 'omni/reference' },
						...referenceGroups([
							['components', 'コンポーネント', 'Components'],
							['lighting', '照明', 'Lighting'],
							['observer', '観察カメラ', 'Observer camera'],
							['logical', 'Logical の型', 'Logical types'],
							['units', '単位の型', 'Unit types'],
							['sequencer', 'Sequencer', 'Sequencer'],
							['celestial', '天体カタログ', 'Celestial catalog'],
							['editor', 'Editor', 'Editor'],
							['shader', 'Shader', 'Shader'],
							['files', 'ファイル', 'Files'],
						]),
					],
				},
			],
		}),
	],
});
