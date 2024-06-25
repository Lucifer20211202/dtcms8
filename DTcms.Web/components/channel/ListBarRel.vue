<template>
	<div class="bar-list">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<div class="list-box">
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
				<template v-if="props.mode==='block'">
					<template v-for="(item,index) in datas.listData" :key="index">
						<NuxtLink :to="common.formatString(props.to, item.id)" class="list-box">
							<div class="img-box">
								<div class="image">
									<el-image :src="common.loadFile(item.imgUrl)" lazy>
										<template #error>
											<i class="iconfont icon-pic"></i>
										</template>
									</el-image>
								</div>
							</div>
							<div class="txt-box">
								<span class="title wrap">{{item.title}}</span>
								<span class="date">{{item.addTime}}</span>
							</div>
						</NuxtLink>
					</template>
				</template>
				<template v-else>
					<template v-for="(item,index) in datas.listData" :key="index">
						<NuxtLink :to="common.formatString(props.to, item.id)" v-if="index==0" class="list-box">
							<div class="img-box">
								<div class="image">
									<el-image :src="common.loadFile(item.imgUrl)" lazy>
										<template #error>
											<i class="iconfont icon-pic"></i>
										</template>
									</el-image>
								</div>
								<div class="label">{{index+1}}</div>
							</div>
							<div class="txt-box">
								<span class="title">{{item.title}}</span>
								<span class="meta">{{item.zhaiyao}}</span>
							</div>
						</NuxtLink>
						<NuxtLink :to="common.formatString(props.to, item.id)" v-else class="list-box">
							<div class="txt-box">
								<div class="title">
									<span class="label">{{index+1}}</span>
									{{item.title}}
								</div>
							</div>
							<span class="view">
								<i class="iconfont icon-view"></i>
								{{item.click}}
							</span>
						</NuxtLink>
					</template>
				</template>
			</template>
		</el-skeleton>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//接收props传值
	const props = defineProps({
		//显示方式(list和block)
		mode: {
			type: String,
			default: () => {
				return 'list'
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
		articleId: 0,
		loading: true,
		listData: [],
	})
	
	//获取路由信息
	const route = useRoute()
	if(route.params.id) {
		datas.articleId = parseInt(route.params.id)
	}
	
	//服务端获取方式
	let sendUrl = `/client/article/view/${datas.articleId}/${props.top}?siteId=${siteConfig.id}`
	if(props.categoryId > 0) {
		sendUrl += `&categoryId=${props.categoryId}`
	}
	if(props.labelId > 0) {
		sendUrl += `&labelId=${props.labelId}`
	}
	if(props.orderBy) {
		sendUrl += `&orderby=${props.orderBy}`
	}
	const {data: listRes} = await useAsyncData(sendUrl, () => useHttp({url: sendUrl}))
	datas.listData = listRes.value?.data
	datas.loading = false
</script>