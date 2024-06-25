<template>
	<div class="text-list">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<div class="list-box radius-box mart-20 pad-20">
					<div class="img-box marr-20" :style="`width:${props.imgWidth};`">
						<div class="image">
							<el-skeleton-item variant="image" style="width:100%;height:100%;" />
						</div>
					</div>
					<div class="txt-box">
						<el-skeleton-item variant="h3" style="margin-top:1rem;width:60%;" />
						<el-skeleton-item variant="h3" style="width:80%;" />
						<el-skeleton-item variant="p" style="margin-bottom:1rem;width:100%;" />
					</div>
				</div>
			</template>
			<template #default>
				<div v-if="datas.listData.length===0" class="nodata">
					<span class="text">暂无投稿内容</span>
				</div>
				<NuxtLink v-for="(item,index) in datas.listData" :key="index"
					:to="common.formatString(props.to, item.id)"
					class="list-box radius-box mart-20 pad-20">
					<div class="img-box marr-20" :style="`width:${props.imgWidth};`">
						<div class="image">
							<img :src="common.loadFile(item.imgUrl)" />
						</div>
					</div>
					<div class="txt-box">
						<div>
							<h3 class="title" :style="`font-size:${props.titleFont};`">{{item.title}}</h3>
							<p class="text" :style="`font-size:${props.infoFont};`">
								{{item.zhaiyao}}
							</p>
						</div>
						<div>
							<div class="mate">
								<div class="left">
									<i class="iconfont icon-date"></i>
									{{item.addTime}}
								</div>
								<div class="right">
									<span class="icon" v-if="item.status==0">待审</span>
									<span class="icon" v-else-if="item.status==1">通过</span>
									<span class="icon" v-else="item.status==2">驳回</span>
								</div>
							</div>
						</div>
					</div>
				</NuxtLink>
			</template>
		</el-skeleton>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//Ref声明
	const divRef = ref(null)
	//接收props传值
	const props = defineProps({
		//显示条数
		top: {
			type: Number,
			default: () => {
				return 10
			}
		},
		//状态
		status: {
			type: Number,
			default: () => {
				return 0
			}
		},
		//链接地址
		to: {
			type: String,
			default: null
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
				return '1rem'
			}
		},
		//摘要字休大小
		infoFont: {
			type: String,
			default: () => {
				return '0.875rem'
			}
		},
	})
	//声明变量
	const datas = reactive({
		loading: true,
		listData: []
	})
	
	//初始化数据
	const initData = () => {
		useHttp({
			url: `/account/article/contribute/view/${props.top}?status=${props.status}&siteId=${siteConfig.id}`,
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
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>