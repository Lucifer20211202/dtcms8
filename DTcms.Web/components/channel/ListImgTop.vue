<template>
	<div ref="divRef" class="image-list">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<el-row :gutter="props.gutter">
					<el-col :xs="props.lays.xs" :sm="props.lays.sm" :md="props.lays.md" :lg="props.lays.lg" v-for="index in props.skeRow">
						<div class="list-box">
							<div class="img-box" :style="`aspect-ratio:${props.mode}`">
								<el-skeleton-item variant="image" style="width:100%;height: 100%;" />
							</div>
							<div class="txt-box">
								<el-skeleton-item variant="h3" style="width:60%" />
								<el-skeleton-item variant="p" style="width:100%" />
							</div>
						</div>
					</el-col>
				</el-row>
			</template>
			<template #default>
				<el-row :gutter="props.gutter">
					<el-col :xs="props.lays.xs" :sm="props.lays.sm" :md="props.lays.md" :lg="props.lays.lg" 
						v-for="(item,index) in datas.listData" :key="index">
						<NuxtLink :to="common.formatString(props.to, item.id)" class="list-box">
							<div class="img-box" :style="`aspect-ratio:${props.mode}`">
								<el-image :src="common.loadFile(item.imgUrl)" lazy>
									<template #error>
										<i class="iconfont icon-pic"></i>
									</template>
								</el-image>
							</div>
							<div class="txt-box">
								<div class="title" v-if="props.show.title">
									<span>{{item.title}}</span>
								</div>
								<div class="info" v-if="props.show.info">
									{{item.zhaiyao}}
								</div>
								<div class="meta" v-if="props.show.date||props.show.click">
									<span v-if="props.show.date">
										<i class="iconfont icon-date"></i>
										{{item.addTime}}
									</span>
									<span v-if="props.show.click">
										<i class="iconfont icon-view"></i>
										{{item.click}}次
									</span>
								</div>
								<div class="foot" v-if="props.show.tag||props.show.like||props.show.msg">
									<div class="tags" v-if="props.show.tag">
										<el-tag v-for="text in item.categoryTitle.split(',')">{{text}}</el-tag>
									</div>
									<div class="icons" v-if="props.show.like||props.show.msg">
										<span class="icon" v-if="props.show.like">
											<i class="iconfont icon-good"></i>
											{{item.likeCount}}
										</span>
										<span class="icon" v-if="props.show.msg">
											<i class="iconfont icon-msg"></i>
											{{item.commentCount}}
										</span>
									</div>
								</div>
							</div>
						</NuxtLink>
					</el-col>
				</el-row>
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
		//链接地址
		to: {
			type: String,
			default: () => {
				return null
			}
		},
		//间隔距离
		gutter: {
			type: Number,
			default: () => {
				return 30
			}
		},
		//骨架数量
		skeRow: {
			type: Number,
			default: () => {
				return 2
			}
		},
		//布局容器
		lays: {
			type: Object,
			default: () => {
				return {
					xs: 24,
					sm: 12,
					md: 8,
					lg: 6
				}
			}
		},
		//显示项
		show: {
			type: Object,
			default: () => {
				return {
					title: true,
					info: false,
					date: false,
					click: false,
					tag: false,
					like: false,
					msg: false,
				}
			}
		},
	})
	//声明变量
	const datas = reactive({
		loading: true,
		listData: []
	})
	
	//初始化数据
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