// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
	runtimeConfig: {
		isServer: true,
		//客户端配置的API地址，在这里修改
		public: {
			baseURL: 'http://localhost:5200',
			timeout: 20000
		}
	},
	vite: {
		build: { chunkSizeWarningLimit: 1600 },
	},
	devtools: { enabled: true },
	modules: [
		'@element-plus/nuxt',
	],
	css: [
		'element-plus/dist/index.css',
		'element-plus/theme-chalk/display.css',
		'animate.css/animate.min.css',
		'@wangeditor/editor/dist/css/style.css',
		'~/assets/fonts/iconfont.css',
		'~/assets/scss/style.scss'
	],
})
