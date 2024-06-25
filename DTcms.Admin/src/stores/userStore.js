import { defineStore } from 'pinia'
import router from "../router"
import http from '../request'

export const useUserStore = defineStore('userStore', {
	state: () => {
		return {
			token: http.getToken(),
		}
	},
	getters: {
		//判断是否登录
		isLoggedIn: (state) => {
			return !!state.token.accessToken
		}
	},
	actions: {
		//this指向state属性，推荐只读，修改使用$patch
		login(token) {
			this.token = token;
			http.setToken(token.accessToken, token.refreshToken)
			router.push({path: '/'})
		},
		logout() {
			this.token = {};
			http.removeToken()
			router.replace({path: '/login'})
		},
	},
})