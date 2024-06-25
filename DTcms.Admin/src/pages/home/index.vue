<template>
	<div class="indexbody">
		<el-container class="main-container">
			<el-aside class="main-left" :width="datas.isCollapse?'46px':'226px'">
				<div class="menu-container" v-loading="datas.menuLoading">
					<transition name="el-fade-in-linear">
						<dt-menu :mini="datas.isCollapse" :data="datas.listData" class="sidebar-nav"></dt-menu>
					</transition>
				</div>
			</el-aside>
			
			<el-container>
				<el-header height="46px" class="main-top">
					<div class="head-nav">
						<div class="head-nav-left">
							<a href="#" @click.prevent="datas.isCollapse=!datas.isCollapse">
								<i v-if="!datas.isCollapse"><Fold /></i>
								<i v-else><Expand /></i>
							</a>
						</div>
						<div class="head-nav-center">
							<div class="logo">
								<img src="../../assets/images/logo.png" />
							</div>
						</div>
						<ul class="head-nav-right">
							<li>
								<div class="avatar">
									<img :src="$api.loadFile(datas.adminInfo.avatar)" v-if="datas.adminInfo.avatar"/>
									<i v-else><UserFilled /></i>
								</div>
								<h3>{{datas.adminInfo.userName}}</h3>
							</li>
							<li>
								<el-dropdown>
									<span class="el-dropdown-link">
										<i><MoreFilled /></i>
									</span>
									<template #dropdown>
										<el-dropdown-menu class="menu-dropdown">
											<el-dropdown-item>
												<el-link :underline="false" icon="Cloudy" :href="datas.config.webUrl" target="_blank">
													预览网站
												</el-link>
											</el-dropdown-item>
											<el-dropdown-item>
												<el-link :underline="false" icon="House" @click="$common.linkUrl('/')">
													管理中心
												</el-link>
											</el-dropdown-item>
											<el-dropdown-item>
												<el-link :underline="false" icon="Lock" @click="$common.linkUrl('/manager/password')">
													修改密码
												</el-link>
											</el-dropdown-item>
											<el-dropdown-item>
												<el-link :underline="false" icon="SwitchButton" @click="handleLogout">
													注销登录
												</el-link>
											</el-dropdown-item>
										</el-dropdown-menu>
									</template>
								</el-dropdown>
							</li>
						</ul>
					</div>
				</el-header>
				<el-main class="main-right">
					<router-view :key="routeKey" v-on:loadMenu="loadMenu" :config="datas.config" :user="datas.adminInfo" v-cloak />
				</el-main>
			</el-container>
		</el-container>
	</div>
</template>

<script setup>
	import { ref,reactive,computed,onMounted,getCurrentInstance,nextTick } from "vue"
	import router from "../../router"
	import { useUserStore } from '../../stores/userStore.js'
	import DtMenu from '../../components/control/DtMenu.vue'
	
	const { proxy } = getCurrentInstance()
	const userStore  = useUserStore()
	
	const datas = reactive({
		isCollapse: true,
		menuLoading: false,
		screenWidth: document.body.clientWidth,
		listData: [],
		config: {
			webName: '',
			webUrl: '',
			webCompany: '',
			webVersion: '',
		},
		adminInfo: {
			lastTime: '',
			lastIp: '',
			addTime: '',
		}
	})
	//计算属性
	const routeKey = computed(() => {
		return router.currentRoute.value.path
	})
	
	//加载菜单
	const loadMenu = async() => {
		//加载菜单列表
		await proxy.$api.request({
			url: '/account/manager/menu',
			beforeSend() {
				datas.menuLoading = true
			},
			success(res) {
				datas.listData = res.data
			},
			complete() {
				datas.menuLoading = false
			}
		});
	}
	//初始化数据
	const initData = async() => {
		//加载菜单列表
		await loadMenu();
		//加载系统信息
		await proxy.$api.request({
			url: '/admin/setting/center/config',
			success(res) {
				datas.config = res.data
			}
		});
		//加载当前管理员信息
		await proxy.$api.request({
			url: '/account/manager/info',
			success(res) {
				datas.adminInfo = res.data
			}
		});
	}
	//展开或隐藏菜单栏
	const pageResize = () => {
		nextTick(() => {
			if(datas.screenWidth > 800){
				datas.isCollapse = false;
			}else{
				datas.isCollapse = true;
			}
		});
	}
	//退出登录
	const handleLogout = () => {
		userStore.logout()
	}
	
	//初始化数据
	initData()
	
	//当页面加载完成事件
	onMounted(() => {
		pageResize()
		window.onresize = () => {
			return (() => {
				datas.screenWidth = window.screenWidth = document.body.clientWidth
				pageResize()
			})()
		}
	})
</script>

<style lang="scss">
	.indexbody {
		width: 100%;
		height: 100%;
		overflow: hidden;
	}
	.main-container {
		height: 100%;
	}
	.main-left {
		display: block;
		position: relative;
		overflow: visible !important;
		.menu-container {
			position: absolute;
			top: 0;
			right: 0;
			bottom: 0;
			left: 0;
			width: 100%;
			.el-scrollbar__wrap {
				overflow-x: hidden;
			}
		}
	}
	.head-nav {
		display: flex;
		justify-content: space-between;
		align-items: center;
		min-width: 285px;
		background: #fff;
		.head-nav-left {
			flex-shrink: 0;
			a {
				display: flex;
				justify-content: center;
				align-items: center;
				width: 46px;
				height: 46px;
				color: #0e70d5;
				text-align: center;
				i {
					font-size: 20px;
					svg{
						height: 20px;
					}
				}
			}
			a:hover {
				color: #409EFF;
			}
		}
		.head-nav-center {
			flex-grow: 1;
			display: flex;
			align-items: center;
			.logo {
				position: relative;
				width: 180px;
				height: 32px;
				opacity: 0.3;
				img {
					max-width: 100%;
					max-height: 100%;
				}
			}
		}
		.head-nav-right {
			flex-shrink: 0;
			li {
				float: left;
				h3 {
					display: inline;
					line-height: 46px;
					color: #555;
					font-size: 14px;
					font-weight: 600;
				}
				.avatar {
					position: relative;
					display: block;
					float: left;
					margin: 8px 5px auto 0;
					width: 30px;
					height: 30px;
					line-height: 30px;
					box-sizing: border-box;
					border: 1px solid #ccc;
					border-radius: 50%;
					background: #fff;
					text-align: center;
					overflow: hidden;
					i {
						color: #ccc;
						font-size: 20px;
						svg{
							width: 20px;
						}
					}
					img {
						width: 100%;
						height: 100%;
					}
				}
				.el-dropdown-link {
					display: flex;
					justify-content: center;
					align-items: center;
					width: 46px;
					height: 46px;
					color: #333;
					text-align: center;
					cursor: pointer;
					&:hover {
						color: #409EFF;
					}
					i {
						font-size: 20px;
						svg{
							height: 20px;
						}
					}
				}
			}
		}
	}
	.main-top,.main-top.el-header {
		margin: 0;
		padding: 0;
		background: #fff;
		box-shadow: 0px 1px 2px 0px rgba(0, 0, 0, 0.05);
		z-index: 2;
	}
	.main-right {
		display: block;
		position: relative;
		padding: 0 !important;
		overflow: hidden !important;
		-webkit-overflow-scrolling: touch !important;
		overflow-y: auto !important;
	}
</style>