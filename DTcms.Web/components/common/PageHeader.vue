<template>
	<el-affix :offset="0">
		<div class="header" :class="{ 'scroll': datas.isScrolled }">
			<div class="wrapper">
				<el-row :gutter="20" justify="space-between">
					<el-col :xs="6" :sm="5" :md="4">
						<div class="logo">
							<img :src="common.loadFile(siteConfig.logo)" v-if="siteConfig.logo" />
							<img v-else src="~/assets/images/logo.png" />
						</div>
					</el-col>
					<el-col :xs="24" :sm="16" :md="16" class="hidden-xs-only">
						<client-only>
						<el-menu mode="horizontal" router class="navbar">
							<template v-for="(item,index) in datas.menuList" :key="index">
								<el-menu-item v-if="item.children.length==0" :index="item.linkUrl">{{item.title}}</el-menu-item>
								<el-sub-menu v-else :index="item.linkUrl">
									<template #title>{{item.title}}</template>
									<template v-for="(sitem,sindex) in item.children" :key="sindex">
										<el-menu-item v-if="sitem.children.length==0" :index="sitem.linkUrl">{{sitem.title}}</el-menu-item>
										<el-sub-menu v-else :index="sitem.linkurl">
											<template #title>{{sitem.title}}</template>
											<template v-for="(citem,cindex) in sitem.children" :key="cindex">
												<el-menu-item :index="citem.linkUrl">{{citem.title}}</el-menu-item>
											</template>
										</el-sub-menu>
									</template>
								</el-sub-menu>	
							</template>
						</el-menu>
						</client-only>
					</el-col>
					<el-col :xs="18" :sm="3" :md="4">
						<div class="navbtn">
							<span class="icon" @click="datas.searchDialog=true">
								<i class="iconfont icon-search"></i>
							</span>
							<NuxtLink class="icon" to="/account">
								<i class="iconfont icon-user"></i>
							</NuxtLink>
							<span class="icon hidden-sm-and-up" @click="datas.menuDrawer=true">
								<el-icon style="transform:rotate(90deg);"><ElIconHistogram /></el-icon>
							</span>
						</div>
					</el-col>
				</el-row>
			</div>
		</div>
	</el-affix>
	<client-only>
		<el-drawer v-model="datas.menuDrawer" size="60%" title="菜单导航">
			<el-menu router @select="datas.menuDrawer=false" class="navbar">
				<template v-for="(item,index) in datas.menuList" :key="index">
					<el-menu-item v-if="item.children.length==0" :index="item.linkUrl">{{item.title}}</el-menu-item>
					<el-sub-menu v-else :index="item.linkUrl">
						<template #title>{{item.title}}</template>
						<template v-for="(sitem,sindex) in item.children" :key="sindex">
							<el-menu-item v-if="sitem.children.length==0" :index="sitem.linkUrl">{{sitem.title}}</el-menu-item>
							<el-sub-menu v-else :index="sitem.linkurl">
								<template #title>{{sitem.title}}</template>
								<template v-for="(citem,cindex) in sitem.children" :key="cindex">
									<el-menu-item :index="citem.linkUrl">{{citem.title}}</el-menu-item>
								</template>
							</el-sub-menu>
						</template>
					</el-sub-menu>	
				</template>
			</el-menu>
		</el-drawer>
		<el-dialog v-model="datas.searchDialog" title="全站搜索" width="60%" :fullscreen="false">
			<common-page-search @close="datas.searchDialog=false" />
		</el-dialog>
	</client-only>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//声明变量
	const datas = reactive({
		isScrolled: false,
		searchDialog: false,
		menuDrawer: false,
		menuList: [],
	})
	//服务端获取方式
	const { data: listRef } = await useAsyncData('sitemenu', () => useHttp({url: '/client/site/menu/1'}))
	datas.menuList = listRef.value?.data
	
	//滚动浮动菜单
	const handleScroll = () => {
		datas.isScrolled = window.scrollY > 0
	}
	onMounted(() => {
		window.addEventListener('scroll', handleScroll)
	})
	onUnmounted(() => {
		window.removeEventListener('scroll', handleScroll)
	})
</script>

<style lang="scss">
	.el-drawer__header {
		margin-bottom: 12px;
	}
	.el-drawer__body {
		padding-top: 0;
	}
	.el-menu--horizontal {
		.el-menu {
			.el-menu-item,
			.el-sub-menu__title {
				margin: 5px;
				color: var(--header-color);
				font-size: 15px;
				&:hover, &:focus {
					background: rgb(0 0 0 / 3%);
					border-radius: 5px;
				}
			}
			.el-menu--popup {
				border-radius: 5px;
			}
		}
	}
	.el-menu--vertical {
		&.el-menu {
			border: none;
			.el-menu-item,
			.el-sub-menu__title {
				&:hover, &:focus {
					border-radius: 5px;
				}
			}
			.el-menu--popup {
				border-radius: 5px;
			}
		}
	}
	.header {
		margin-bottom: 20px;
		padding: 0;
		transition: .3s;
		color: var(--header-color);
		background: var(--header-bg);
		box-shadow: 0 4px 10px var(--main-shadow);
		opacity: 0.9;
		.el-row {
			align-items: center;
			height: 3.75rem;
		}
		.logo {
			display: flex;
			align-items: center;
			img {
				width: 100%;
				max-width: 150px;
			}
		}
		.navbar {
			height: 3.75rem;
			background: none;
			&.el-menu {
				border: none;
				.el-menu-item,
				.el-sub-menu__title {
					color: var(--header-color);
					font-size: var(--header-font);
					border-bottom: 0;
					&:hover,
					&:focus {
						background: none;
					}
				}
				.el-sub-menu {
					&.is-active {
						.el-sub-menu__title {
							border-bottom: 0;
						}
					}
				}
			}
		}
		.navbtn {
			display: flex;
			justify-content: end;
			align-items: center;
			.icon {
				cursor: pointer;
				display: flex;
				align-items: center;
				padding: 5px 10px;
				i {
					font-size: var(--header-font);
				}
			}
		}
	}
</style>