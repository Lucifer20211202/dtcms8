<template>
	<div ref="divRef">
		<el-carousel class="slide-box" :interval="props.interval">
			<el-carousel-item class="item-box" v-for="(item,index) in datas.listData.banners">
				<div class="img-box">
					<img :src="common.loadFile(item.filePath)" />
				</div>
				<div class="wrapper">
					<div class="txt-box">
						<h3 class="animate__animated animate__fadeInDown">{{item.title}}</h3>
						<p class="animate__animated animate__fadeIn animate__delay-1s">{{item.content}}</p>
						<a :href="item.linkUrl" class="animate__animated animate__fadeInUp animate__delay-2s link">点击查看更多..</a>
					</div>
				</div>
			</el-carousel-item>
		</el-carousel>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//Ref声明
	const divRef = ref(null)
	//接收props传值
	const props = defineProps({
		interval: {
			type: Number,
			default: () => {
				return 3000
			}
		},
		index: {
			type: String,
			default: () => {
				return 'home'
			}
		}
	})
	//声明变量
	const datas = reactive({
		loading: true,
		listData: {
			banners:[]
		}
	})
	
	//客户端获取数据
	const initData = async() => {
		useHttp({
			url: `/client/advert/${props.index}?siteId=${siteConfig.id}`,
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
	
	//获取广告位信息（服务端获取方式）
	/*const {data:advertData} = await useAsyncData('advert', () => useHttp({url: `/client/advert/${props.index}?siteId=${siteConfig.id}`}))
	datas.listData = advertData.value.data*/
</script>

<style lang="scss">
	.slide-box {
		height: calc(100vw*0.4);
		background: #ccc;
		.el-carousel__indicators--horizontal {
			bottom: calc(10vw);
		}
		.wrapper {
			height: 100%;
		}
		.item-box {
			height: calc(100vw*0.4);
		}
		.img-box {
			top: 0;
			left: 0;
			position: absolute;
			width: 100%;
			height: 100%;
			img {
				width: 100%;
				min-height: 100%;
			}
		}
		.txt-box {
			top: 68px;
			left: 20px;
			right: 20px;
			bottom: calc(10vw);
			position: absolute;
			text-align: center;
			opacity: 0.9;
			h3 {
				margin-top: 2.2em;
				color: #fff;
				font-size: 2.5vw;
				font-weight: 400;
				text-align: center;
				line-height: 1em;
			}
			p {
				margin-top: 1em;
				color: #757d8e;
				font-size: 1.5vw;
				font-weight: 400;
				text-align: center;
				line-height: 1.5em;
				word-break: break-all;
				display: -webkit-box;
				-webkit-line-clamp: 2;
				-webkit-box-orient: vertical;
				overflow: hidden;
			}
			.link {
				display: inline-block;
				margin-top: 1em;
				color: #fff;
				font-size: 1.2vw;
				font-weight: 300;
				line-height: 1em;
				padding: 0.5em 1em;
				border: 1px solid rgb(250 250 250 / 80%);
				border-radius: 1.5em;
				cursor: pointer;
				transition: all .3s ease-in-out;
				&:hover {
					box-shadow: 0 0 0.5em 0 #fff;
				}
			}
		}
	}
</style>