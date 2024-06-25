<template>
	<div v-for="(item, index) in datas.listData">
		<slot :item="item"></slot>
	</div>
</template>

<script setup>
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
		//类别父ID
		parentId: {
			type: Number,
			default: () => {
				return 0
			}
		},
		//显示条数
		top: {
			type: Number,
			default: () => {
				return 10
			}
		},
		
	})
	//声明变量
	const datas = reactive({
		listData: []
	})
	
	//加载分类
	const {data:categoryRes} = await useAsyncData('categoryList', () => useHttp({
		url: `/client/article/category/channel/${props.channel}/view/${props.parentId}/${props.top}?siteId=${siteConfig.id}`
	}))
	datas.listData = categoryRes.value?.data
</script>