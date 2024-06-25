<template>
	<div ref="divRef" class="image-list">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<el-row :gutter="props.gutter" class="marb-30">
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
		<div class="nodata" v-if="!datas.loading&&datas.listData.length==0">暂无数据显示..</div>
	</div>
	<div class="pager-box" v-if="datas.totalCount>0">
		<el-pagination background layout="prev, pager, next"
			:page-size="props.pageSize"
			:current-page="datas.pageIndex"
			:total="datas.totalCount"
			@current-change="handleCurrentChange" />
	</div>
</template>

<script setup>
	//Ref声明
	const divRef = ref(null)
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
		//查询关健字
		keyword: {
			type: String,
			default: () => {
				return null
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
				return 20
			}
		},
		//每页数量
		pageSize: {
			type: Number,
			default: () => {
				return 10
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
		totalCount: 0, //总数量
		pageIndex: 1, //当前页码
		listData: []
	})
	
	//初始化数据
	const initData = async() => {
		await nextTick() //等待父组件传值，否则出错
		let sendUrl = `/client/article/channel/${props.channel}?siteId=${siteConfig.id}&pageSize=${props.pageSize}&pageIndex=${datas.pageIndex}`
		if(props.categoryId > 0) {
			sendUrl += `&categoryId=${props.categoryId}`
		}
		if(props.labelId > 0) {
			sendUrl += `&labelId=${props.labelId}`
		}
		if(props.orderBy) {
			sendUrl += `&orderby=${props.orderBy}`
		}
		if(props.keyword) {
			sendUrl += `&keyword=${props.keyword}`
		}
		//发送请求
		await useHttp({
			url: sendUrl,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data
				datas.totalCount = res.pagination.totalCount
				datas.pageIndex = res.pagination.pageIndex
			},
			error(err) {
				datas.listData = []
			},
			complete() {
				datas.loading = false
			},
		})
	}
	//跳转到第几页
	const handleCurrentChange = (val) => {
		if (datas.pageIndex != val) {
			datas.pageIndex = val
			initData()
		}
	}
	
	//使用ref暴露方法给父组件
	defineExpose({
		initData
	})
	//获取当前站点信息(必须在defineExpose后面，否则无法暴露)
	const siteConfig = await useSite('site')
</script>