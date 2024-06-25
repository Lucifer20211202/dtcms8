<template>
	<div class="wrapper">
		<div class="news-focus">
			<el-row :gutter="30" justify="space-between">
				<el-col :xs="24" :sm="24" :md="12" :lg="14">
					<channel-list-text-focus channel="news" :top="5" mode="4/3" imgWidth="25%" to="/news/show/{0}" />
				</el-col>
				<el-col :xs="24" :sm="24" :md="12" :lg="10" class="hidden-sm-and-down">
					<channel-list-banner channel="news" :labelId="1" :top="8" :interval="5000" to="/news/show/{0}" />
				</el-col>
			</el-row>
		</div>
	</div>
	
	<div class="wrapper mart-30 padb-30">
		<!--分类列表-->
		<channel-list-category-top channel="news" :parentId="0" :top="8">
			<template #default="data">
				<!--标题-->
				<div class="tabs-nav-item">
					<div class="title">{{data.item.title}}</div>
					<NuxtLink :to="`/news/list/${data.item.id}`" class="more">
						更多
						<i class="iconfont icon-right-arrow"></i>
					</NuxtLink>
				</div>
				<!--列表-->
				<channel-list-text-top
					channel="news"
					:categoryId="data.item.id"
					:top="6"
					mode="4/3"
					imgWidth="30%"
					to="/news/show/{0}"
					:skeRow="2"
					:show="{title:true,info:true,click:true,msg:true,like:true,date:true}" />
			</template>
		</channel-list-category-top>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: '新闻资讯 - ' + siteConfig.seoTitle,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
</script>