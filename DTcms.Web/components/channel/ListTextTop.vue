<template>
	<div ref="divRef" class="text-list" :style="`margin-top:-${props.gutter}px;`">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<el-row :gutter="props.gutter">
					<el-col :xs="props.lays.xs" :sm="props.lays.sm" :md="props.lays.md" :lg="props.lays.lg"
						v-for="index in props.skeRow" :key="index" :style="`margin-top:${props.gutter}px;`">
						<div class="list-box radius-box mart-20 pad-20">
							<div class="txt-box">
								<el-skeleton-item variant="h3" style="margin-top:1rem;width:60%;" />
								<el-skeleton-item variant="h3" style="margin:1rem 0;width:100%;" />
								<el-skeleton-item variant="p" style="margin-bottom:1rem;width:80%;" />
							</div>
						</div>
					</el-col>
				</el-row>
			</template>
			<template #default>
				<el-row :gutter="props.gutter">
					<el-col :xs="props.lays.xs" :sm="props.lays.sm" :md="props.lays.md" :lg="props.lays.lg"
						v-for="(item,index) in datas.listData" :key="index" :style="`margin-top:${props.gutter}px;`">
						<NuxtLink :to="common.formatString(props.to, item.id)" class="list-box radius-box pad-20">
							<div v-if="item.imgUrl" class="img-box marr-20" :style="`width:${props.imgWidth};`">
								<div class="image" :style="`aspect-ratio:${props.mode};`">
									<el-image :src="common.loadFile(item.imgUrl)" lazy>
										<template #error>
											<i class="iconfont icon-pic"></i>
										</template>
									</el-image>
								</div>
							</div>
							<div class="txt-box">
								<div>
									<h3 v-if="props.show.title" class="title" :style="`font-size:${props.titleFont};`">
										{{item.title}}
									</h3>
									<p v-if="props.show.info" class="text" :style="`font-size:${props.infoFont};`">
										{{item.zhaiyao}}
									</p>
								</div>
								<div>
									<div class="tags" v-if="props.show.tag">
										<el-tag v-for="text in item.labelTitle.split(',')">{{text}}</el-tag>
									</div>
									<div class="mate" v-if="props.show.date||props.show.msg||props.show.click||props.show.like">
										<div v-if="props.show.date" class="left">
											<i class="iconfont icon-date"></i>
											{{item.addTime}}
										</div>
										<div class="right">
											<span v-if="props.show.msg" class="icon">
												<i class="iconfont icon-message"></i>
												{{item.commentCount}}
											</span>
											<span v-if="props.show.click" class="icon">
												<i class="iconfont icon-view"></i>
												{{item.click}}
											</span>
											<span v-if="props.show.like" class="icon">
												<i class="iconfont icon-good"></i>
												{{item.likeCount}}
											</span>
										</div>
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
		//图片宽度
		imgWidth: {
			type: String,
			default: () => {
				return '30%'
			}
		},
		//标题字体大小
		titleFont: {
			type: String,
			default: () => {
				return '1.125rem'
			}
		},
		//摘要字休大小
		infoFont: {
			type: String,
			default: () => {
				return '0.875rem'
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
				return 20
			}
		},
		//骨架数量
		skeRow: {
			type: Number,
			default: () => {
				return 1
			}
		},
		//布局容器
		lays: {
			type: Object,
			default: () => {
				return {
					xs: 24,
					sm: 12,
					md: 12,
					lg: 12
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
					click: false,
					msg: false,
					like: false,
					date: false,
					tag: false,
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
</script>