<template>
	<div class="wrapper">
		<el-row :gutter="30" justify="space-between">
			<el-col :xs="24" :sm="24" :md="12" class="hidden-sm-and-down">
				<channel-list-banner channel="video" :labelId="1" :top="8" to="/video/show/{0}" />
			</el-col>
			<el-col :xs="24" :sm="24" :md="12">
				<channel-list-img-top channel="video" mode="4/3" :top="2" to="/video/show/{0}" :lays="{xs:24, sm:12}" :show="{title:true, tag:true, like:true, msg:true}" />
			</el-col>
		</el-row>
		<div class="mart-30 marb-30">
			<!--分类列表-->
			<channel-list-category-top channel="video" :parentId="0" :top="8">
				<template #default="data">
					<!--标题-->
					<div class="tabs-nav-item">
						<div class="title">{{data.item.title}}</div>
						<NuxtLink :to="`/video/list/${data.item.id}`" class="more">
							更多
							<i class="iconfont icon-right-arrow"></i>
						</NuxtLink>
					</div>
					<!--列表-->
					<channel-list-img-top 
						channel="video"
						:categoryId="data.item.id"
						:top="8"
						mode="16/9"
						imgWidth="30%"
						to="/video/show/{0}"
						:skeRow="4"
						:show="{title:true, info:true, date:true, click:true}" />
				</template>
			</channel-list-category-top>
		</div>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: '视频中心 - ' + siteConfig.seoTitle,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
</script>