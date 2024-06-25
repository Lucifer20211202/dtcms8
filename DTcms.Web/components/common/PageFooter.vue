<template>
	<div class="footer">
		<div class="wrapper">
			<el-row :gutter="20" justify="space-between">
				<el-col :xs="8" :sm="5" :md="5" class="logo">
					<img :src="common.loadFile(siteConfig.logo)" v-if="siteConfig.logo" />
					<img v-else src="~/assets/images/logo.png" />
				</el-col>
				<el-col :xs="24" :sm="12" :md="11" class="hidden-xs-only">
					<el-row :gutter="20" justify="space-between">
						<el-col :span="8" class="list-box" v-for="(item,index) in datas.contentList" :key="index">
							<h4 class="title">{{item.title}}</h4>
							<div class="text" v-for="(citem,cindex) in item.data" :key="cindex">
								<NuxtLink :to="`/content/show/${citem.callIndex?citem.callIndex:citem.id}`">{{citem.title}}</NuxtLink>
							</div>
						</el-col>
					</el-row>
				</el-col>
				<el-col :xs="16" :sm="7" :md="8">
					<div class="list-box">
						<h4 class="title">联系我们</h4>
						<div class="text">
							<span>电话：</span>
							<span class="info">{{siteConfig.tel}}</span>
						</div>
						<div class="text">
							<span>地址：</span>
							<span class="info">{{siteConfig.address}} 更多请参见“关于我们”</span>
						</div>
						<div class="text">
							<span>邮箱：</span>
							<span class="info">{{siteConfig.email}}</span>
						</div>
						<div class="text">
							<span>给我留言</span>
						</div>
					</div>
				</el-col>
			</el-row>
			<div class="icon-wrap">
				<div class="icon-box">
					<el-popover placement="top" trigger="hover">
						<template #reference>
							<i class="iconfont icon-qq-full"></i>
						</template>
						<img src="~/assets/images/qq.jpg" width="120" height="120" />
					</el-popover>
					<el-popover placement="top" trigger="hover">
						<template #reference>
							<i class="iconfont icon-weixin-full"></i>
						</template>
						<img src="~/assets/images/weixin.jpg" width="120" height="120" />
					</el-popover>
					<el-popover placement="top" trigger="hover">
						<template #reference>
							<i class="iconfont icon-douyin-full"></i>
						</template>
						<img src="~/assets/images/douyin.jpg" width="120" height="120" />
					</el-popover>
				</div>
				<div class="btn-box">
					<a target="_blank" href="http://www.dtcms.net">
						<el-icon><ElIconDownload /></el-icon>
						<span>下载源码</span>
					</a>
				</div>
			</div>
			<div class="copyright">{{siteConfig.copyright}}
				<a target="_blank" href="https://beian.miit.gov.cn">{{siteConfig.crod}}</a>
			</div>
		</div>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//声明变量
	const datas = reactive({
		contentList: []
	})
	//服务端获取方式
	const { data: listRef } = await useAsyncData('footerContent', () => useHttp({
		url: `/client/article/channel/content/view/4/8?siteId=${siteConfig.id}`
	}))
	if(listRef.value) {
		datas.contentList = listRef.value?.data
	}
</script>

<style lang="scss">
	.footer {
		color: var(--footer-color);
		padding: 50px 0 10px;
		background: var(--footer-bg);
		.logo {
			img {
				width: 100%;
				max-width: 150px;
				max-height:32px;
				filter:brightness(100)
			}
		}
		.list-box {
			line-height: 2em;
			.title {
				margin: 0 0 24px 0;
				font-weight: 600;
				font-size: 1rem;
			}
			.text {
				display: flex;
				flex-direction: row;
				color: #fff;
				font-size: 0.92rem;
				a {
					color: #fff;
					&:hover {
						color: rgba(255, 255, 255, 0.8);
					}
				}
				.info {
					flex: 1 1 0%;
				}
			}
		}
		.icon-wrap {
			display: flex;
			justify-content: space-between;
			margin-top: 30px;
			.icon-box {
				display: flex;
				align-items: center;
				i {
					padding: 10px;
					font-size: 20px;
					cursor: pointer;
				}
			}
			.btn-box {
				.el-button {
					color: #fff;
					border: 1px solid #4c5059;
					background: none;
				}
				a {
					display: inline-block;
					padding: 8px 15px;
					color: #fff;
					font-size: 14px;
					line-height: 1em;
					border-radius: 3px;
					border: 1px solid #4c5059;
					align-items: center;
					overflow: hidden;
					&:hover {
						color: rgba(255, 255, 255, 0.8);
					}
					span {
						margin-left: 6px;
					}
				}
			}
		}
		.copyright {
			text-align: center;
			margin: 20px;
			color: #8e97ae;
			font-size: 0.875rem;
			font-weight: 400;
			a {
				color: #8e97ae;
				&:hover {
					color: #fff;
				}
			}
		}
	}
</style>