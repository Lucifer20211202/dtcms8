<template>
	<div ref="divRef" class="banner-box">
		<el-skeleton :loading="datas.loading" style="height:100%;" animated>
			<template #template>
				<el-skeleton-item variant="image" style="height:100%;" />
			</template>
			<template #default>
				<el-carousel :interval="props.interval" height="100%" style="height:100%;">
					<el-carousel-item v-for="(item,index) in datas.listData" :key="index" style="height:100%;">
						<NuxtLink :to="common.formatString(props.to, item.id)" class="list-box">
							<img :src="common.loadFile(item.imgUrl)" />
							<div class="title">{{item.title}}</div>
						</NuxtLink>
					</el-carousel-item>
				</el-carousel>
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
		//轮换时间
		interval: {
			type: Number,
			default: () => {
				return 3000
			}
		},
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
	const { data: listRef } = await useAsyncData(sendUrl, () => useHttp({url: sendUrl}))
	datas.listData = listRef.value?.data
	datas.loading = false*/
</script>

<style lang="scss">
	.banner-box {
		position: relative;
		border-radius: var(--main-radius);
		height: 100%;
		overflow: hidden;
		.list-box {
			display: flex;
			justify-items: center;
			align-items: center;
			position: relative;
			height: 100%;
			&:hover {
				.el-image, img {
					transform: scale(1.2);
				}
			}
			.el-image, img {
				width: 100%;
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
			.title {
				left: 0;
				bottom: 20px;
				right: 0;
				position: absolute;
				display: block;
				padding: 5px 20px;
				color: #fff;
				font-size: 1.5rem;
				line-height: 1.5em;
				word-break: break-all;
				display: -webkit-box;
				-webkit-line-clamp: 1;
				-webkit-box-orient: vertical;
				text-shadow: 2px 2px 5px #333;
				overflow: hidden;
			}
		}
	}
</style>