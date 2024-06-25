<template>
	<div class="count-box marb-20 pad-20 even radius-box">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<el-skeleton-item variant="h3" style="margin-top:0.5rem;width:60%;" />
				<el-skeleton-item variant="h3" style="margin:0.5rem 0;width:100%;" />
			</template>
			<NuxtLink :to="props.to" class="title">
				{{props.title}}
				<i class="iconfont icon-right-arrow"></i>
			</NuxtLink>
			<div class="text">
				<div class="num"><b>{{userInfo.point}}</b>分</div>
				<i class="icon iconfont icon-point"></i>
			</div>
		</el-skeleton>
	</div>
</template>

<script setup>
	//获取当前用户信息
	const userInfo = await useUser('info')
	//接收props传值
	const props = defineProps({
		//标题
		title: {
			type: String,
			default: '积分'
		},
		//链接地址
		to: {
			type: String,
			default: null
		},
	})
	
	//声明变量
	const datas = reactive({
		loading: true,
	})
	
	//页面完成后执行
	onMounted(() => {
		datas.loading = false
	})
</script>