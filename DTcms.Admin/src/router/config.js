import Home from '../pages/home/index.vue'
import Login from '../pages/login/index.vue'

export default {
	routes: [
		{
			path:'/',
			component: Home,
			children: [
				{
					path: '/',
					props: true,
					component: () => import('../pages/home/center.vue'),
					meta: { 
						title:'管理中心',
						requiresAuth: true,
					}
				},
				{
					path: '/setting/config',
					component: () => import('../pages/setting/config.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/site/list',
					component: () => import('../pages/setting/site/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/site/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/setting/site/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/menu/list',
					component: () => import('../pages/setting/menu/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/menu/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/setting/menu/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/channel/list',
					component: () => import('../pages/setting/channel/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/channel/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/setting/channel/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/area/list',
					component: () => import('../pages/setting/area/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/area/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/setting/area/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/oauth/list',
					component: () => import('../pages/setting/oauth/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/oauth/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/setting/oauth/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/oauth/login',
					component: () => import('../pages/setting/oauth/login.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/payment/list',
					component: () => import('../pages/setting/payment/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/payment/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/setting/payment/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/payee/list',
					component: () => import('../pages/setting/payee/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/setting/payee/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/setting/payee/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				//管理员
				{
					path: '/manager/password',
					component: () => import('../pages/manager/password.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/manager/list',
					component: () => import('../pages/manager/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/manager/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/manager/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/manager/menu/list',
					component: () => import('../pages/manager/menu/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/manager/menu/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/manager/menu/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/manager/role/list',
					component: () => import('../pages/manager/role/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/manager/role/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/manager/role/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/manager/log/list',
					component: () => import('../pages/manager/log/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				//会员
				{
					path: '/member/config',
					component: () => import('../pages/member/config.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/audit',
					component: () => import('../pages/member/audit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/list',
					component: () => import('../pages/member/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/edit/:id?',
					props: true,
					component: () => import('../pages/member/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/group/list',
					component: () => import('../pages/member/group/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/group/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/member/group/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/template/list',
					component: () => import('../pages/member/template/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/template/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/member/template/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/message/list',
					component: () => import('../pages/member/message/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/message/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/member/message/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/recharge/list',
					component: () => import('../pages/member/recharge/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/recharge/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/member/recharge/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/balance/list',
					component: () => import('../pages/member/balance/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/balance/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/member/balance/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/point/list',
					component: () => import('../pages/member/point/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/member/point/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/member/point/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				//文章
				{
					path: '/article/list/:channelId(\\d+)',
					props: true,
					component: () => import('../pages/article/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/edit/:channelId(\\d+)/:id(\\d+)?',
					props: true,
					component: () => import('../pages/article/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/category/list/:channelId(\\d+)',
					props: true,
					component: () => import('../pages/article/category/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/category/edit/:channelId(\\d+)/:id(\\d+)?',
					props: true,
					component: () => import('../pages/article/category/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/label/list/:channelId(\\d+)',
					props: true,
					component: () => import('../pages/article/label/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/label/edit/:channelId(\\d+)/:id(\\d+)?',
					props: true,
					component: () => import('../pages/article/label/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/contribute/list/:channelId(\\d+)',
					props: true,
					component: () => import('../pages/article/contribute/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/contribute/edit/:channelId(\\d+)/:id(\\d+)?',
					props: true,
					component: () => import('../pages/article/contribute/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/article/comment/list/:channelId(\\d+)',
					props: true,
					component: () => import('../pages/article/comment/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				//应用
				{
					path: '/apply/link/list',
					component: () => import('../pages/apply/link/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/apply/link/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/apply/link/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/apply/feedback/list',
					component: () => import('../pages/apply/feedback/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/apply/feedback/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/apply/feedback/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/apply/advert/list',
					component: () => import('../pages/apply/advert/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/apply/advert/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/apply/advert/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/apply/advert/banner/list',
					component: () => import('../pages/apply/advert/banner/list.vue'),
					meta: {
						requiresAuth: true,
					}
				},
				{
					path: '/apply/advert/banner/edit/:id(\\d+)?',
					props: true,
					component: () => import('../pages/apply/advert/banner/edit.vue'),
					meta: {
						requiresAuth: true,
					}
				},
			]
		},
		{
			path:'/login',
			name: 'Login',
			component: Login,
			meta: {
				title:'管理员登录',
				requiresAuth: false,
			}
		},
		{
			path: '/:catchAll(.*)',
			name: '404',
			component: () => import('../pages/error/404.vue'),
			meta: {
				title:'404页面不存在',
				requiresAuth: false,
			}
		},
	]
}