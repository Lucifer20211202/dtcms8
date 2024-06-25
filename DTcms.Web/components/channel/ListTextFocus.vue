<template>
	<div ref="divRef" class="hot-list">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<div class="list-box top">
					<div class="img-box">
						<div class="image">
							<el-skeleton-item variant="image" style="width:100%;height:100%;" />
						</div>
					</div>
					<div class="txt-box">
						<el-skeleton-item variant="p" style="width:50%;" />
						<el-skeleton-item variant="p" style="width:100%;" />
						<el-skeleton-item variant="p" style="width:80%;" />
					</div>
				</div>
				<div class="list-box">
					<div class="txt-box">
						<el-skeleton-item variant="p" style="margin-top:2rem;width:100%;" />
						<el-skeleton-item variant="p" style="margin:1.25rem 0;width:80%;" />
						<el-skeleton-item variant="p" style="width:100%;" />
					</div>
				</div>
			</template>
			<template #default>
				<template v-for="(item,index) in datas.listData" :key="index">
					<NuxtLink :to="common.formatString(props.to, item.id)" v-if="index==0" class="list-box top">
						<div class="img-box" :style="`width:${props.imgWidth};`">
							<div class="image" :style="`aspect-ratio:${props.mode};`">
								<el-image :src="common.loadFile(item.imgUrl)" lazy>
									<template #error>
										<i class="iconfont icon-pic"></i>
									</template>
								</el-image>
							</div>
						</div>
						<div class="txt-box">
							<span class="title">{{item.title}}</span>
							<span class="meta">{{item.zhaiyao}}</span>
							<span class="info">
								<div class="tags">
									<el-tag v-for="title in item.categoryTitle.split(',')">{{title}}</el-tag>
								</div>
								<div class="time">{{item.addTime}}</div>
							</span>
						</div>
					</NuxtLink>
					<NuxtLink :to="common.formatString(props.to, item.id)" v-else class="list-box">
						<div class="txt-box">
							<div class="title">
								<span class="label">{{item.labelTitle}}</span>
								{{item.title}}
							</div>
						</div>
						<span class="date">{{item.addTime}}</span>
					</NuxtLink>
				</template>
			</template>
		</el-skeleton>
	</div>
</template>

<script setup>
	//Ref声明
	const divRef = ref(null)
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//接收props传值
	const props = defineProps({
		//频道名称
		channel: {
			type: String,
			default: () => {
				return null
			}
		},
		//类别ID
		categoryId: {
			type: Number,
			default: () => {
				return 0
			}
		},
		//标签ID
		labelId: {
			type: Number,
			default: () => {
				return 0
			}
		},
		//排序
		orderBy: {
			type: String,
			default: () => {
				return null
			}
		},
		//显示条数
		top: {
			type: Number,
			default: () => {
				return 10
			}
		},
		//缩略图宽高比
		mode: {
			type: String,
			default: () => {
				return '4/3'
			}
		},
		//图片宽度
		imgWidth: {
			type: String,
			default: () => {
				return '25%'
			}
		},
		//链接地址
		to: {
			type: String,
			default: () => {
				return null
			}
		},
	})
	//声明变量
	const datas = reactive({
		loading: true,
		listData: []
	})
	
	//客户端获取数据
	const initData = async() => {
		let sendUrl = `/client/article/channel/${props.channel}/view/${props.top}?siteId=${siteConfig.id}`
		if(props.categoryId > 0) {
			sendUrl += `&categoryId=${props.categoryId}`
		}
		if(props.labelId > 0) {
			sendUrl += `&labelId=${props.labelId}`
		}
		if(props.orderBy) {
			sendUrl += `&orderby=${props.orderBy}`
		}
		useHttp({
			url: sendUrl,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data
			},
			complete() {
				datas.loading = false
			}
		})
	}
	//进入可视区才加载数据
	const initObserver = () => {
		const observer = new IntersectionObserver(([entry]) => {
			if (entry.isIntersecting) {
				initData() //进入可视区加载数据
				observer.unobserve(divRef.value) //加载完成后，停止观察
			}
		}, { threshold: 0 }) //threshold 设置为 1.0 表示目标元素完全进入可视区时触发回调
		//开始观察是否进入可视区
		if (divRef.value) {
			observer.observe(divRef.value)
		}
	}
	//页面完成后执行
	onMounted(() => {
		initObserver()
	})
	
	//服务端获取方式
	/*let sendUrl = `/client/article/channel/${props.channel}/view/${props.top}?siteId=${siteConfig.id}`
	if(props.categoryId > 0) {
		sendUrl += `&categoryId=${props.categoryId}`
	}
	if(props.labelId > 0) {
		sendUrl += `&labelId=${props.labelId}`
	}
	if(props.orderBy) {
		sendUrl += `&orderby=${props.orderBy}`
	}
	const {data:listRes} = await useAsyncData(sendUrl, () => useHttp({url: sendUrl}))
	datas.listData = listRes.value?.data
	datas.loading = false*/
</script>

<style lang="scss">
	.hot-list {
		margin-top: -1.25rem;
		.list-box {
			display: flex;
			margin-top: 1.25rem;
			&:hover {
				.img-box {
					.el-image, img {
						transform: scale(1.2);
					}
				}
			}
			&.top {
				.txt-box {
					.title {
						font-size: 1.5rem;
					}
				}
			}
			.img-box {
				flex-shrink: 0;
				margin-right: 1.875rem;
				min-width: 5rem;
				.image {
					display: flex;
					justify-content: center;
					align-items: center;
					width: 100%;
					background: rgba(0, 0, 0, 0.03);
					border-radius: var(--main-radius);
					overflow: hidden;
					.el-image, img {
						min-width: 100%;
						min-height: 100%;
						object-fit: cover;
						transition: all 0.5s;
						background: rgb(0 0 0 / 3%);
						display: flex;
						justify-content: center;
						align-items: center;
						i {
							color: var(--muted-3-color);
							font-size: 2rem;
						}
					}
				}
			}
			.txt-box {
				flex-grow: 1;
				display: flex;
				flex-direction: column;
				justify-content: space-between;
				align-items: stretch;
				overflow: hidden;
				.title {
					font-size: 1rem;
					text-overflow: ellipsis;
					white-space: nowrap;
					overflow: hidden;
					.label {
						display: inline-block;
						margin-right: 2px;
						padding: 3px 5px;
						color: #fff;
						font-size: 0.75rem;
						line-height: 1em;
						background: #e93323;
						border-radius: 3px;
					}
				}
				.meta {
					margin: 1rem 0;
					color: var(--muted-color);
					font-size: 0.875rem;
					word-break: break-all;
					display: -webkit-box;
					-webkit-line-clamp: 2;
					-webkit-box-orient: vertical;
					overflow: hidden;
				}
				.info {
					display: flex;
					justify-content: space-between;
					align-items: center;
					color: var(--muted-3-color);
					font-size: 0.875rem;
					.tags {
						margin-left: -5px;
						text-overflow: ellipsis;
						white-space: nowrap;
						overflow: hidden;
						.el-tag {
							margin-left: 5px;
						}
					}
					.time {
						font-size: 0.875rem;
						text-overflow: ellipsis;
						white-space: nowrap;
						overflow: hidden;
					}
				}
			}
			.date {
				color: var(--muted-3-color);
				font-size: 0.875rem;
				text-overflow: ellipsis;
				white-space: nowrap;
				overflow: hidden;
			}
		}
	}
</style>