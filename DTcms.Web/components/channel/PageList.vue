<template>
	<div>
		<slot :datas="datas">
		</slot>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//声明变量
	const datas = reactive({
		categoryId: 0, //分类ID
		labelId: 0, //标签ID
		keyword: null, //查询关健字
		model: {}, //当前分类信息
	})
	//获取路由信息
	const route = useRoute()
	if(route.params.id) {
		datas.categoryId = parseInt(route.params.id)
	}
	if(route.query.labelId) {
		datas.labelId = parseInt(route.query.labelId)
	}
	//关健字
	if (route.query.keyword) {
		datas.keyword = route.query.keyword
	}
	
	//服务端获取当前分类
	if(datas.categoryId > 0) {
		const {data: infoRes} = await useAsyncData('categoryInfo', () => useHttp({url: `/client/article/category/${datas.categoryId}`}))
		datas.model = infoRes.value?.data
	}
	
	//页面SEO设置
	const seoTitle = datas.model?.seoTitle ? datas.model?.seoTitle : `${siteConfig.seoTitle}`
	const seoKeyword = datas.model?.seoKeyword ? datas.model.seoKeyword : siteConfig.seoKeyword
	const seoDescription = datas.model?.seoDescription ? datas.model.seoDescription : siteConfig.seoDescription
	useSeoMeta({
		title: seoTitle,
		ogTitle: seoKeyword,
		description: seoDescription,
		ogDescription: seoDescription,
	})
</script>