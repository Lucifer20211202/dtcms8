<template>
	<div class="wrapper">
		<el-row :gutter="30" justify="space-between">
			<el-col :xs="24" :sm="24" :md="12" class="hidden-sm-and-down">
				<channel-list-banner channel="photo" :labelId="1" :top="8" to="/photo/show/{0}" />
			</el-col>
			<el-col :xs="24" :sm="24" :md="12">
				<channel-list-img-top channel="photo" mode="4/3" :top="2" to="/photo/show/{0}" :lays="{xs:24, sm:12}" :show="{title:true, tag:true, like:true, msg:true}" />
			</el-col>
		</el-row>
		<div class="mart-30 marb-30">
			<!--分类列表-->
			<channel-list-category-top channel="photo" :parentId="0" :top="8">
				<template #default="data">
					<!--标题-->
					<div class="tabs-nav-item">
						<div class="title">{{data.item.title}}</div>
						<NuxtLink :to="`/photo/list/${data.item.id}`" class="more">
							更多
							<i class="iconfont icon-right-arrow"></i>
						</NuxtLink>
					</div>
					<!--列表-->
					<channel-list-img-top 
						channel="photo"
						:categoryId="data.item.id"
						:top="8"
						mode="4/3"
						to="/photo/show/{0}"
						:skeRow="4"
						:show="{title:true, tag:true, like:true, msg:true}" />
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
		title: '图片分享 - ' + siteConfig.seoTitle,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
</script>