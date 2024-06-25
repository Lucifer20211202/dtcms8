<template>
	<!--幻灯片-->
	<section class="index-slide">
		<advert-home-slide :interval="5000" index="home" />
	</section>
	<!--资讯模块-->
	<section class="index-news">
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
	</section>
	<!--视频中心-->
	<section>
		<div class="wrapper">
			<div class="title-box">
				<h3>视频中心</h3>
				<p class="text">关注行业动态，跟进时代发展，不断创新</p>
			</div>
			<channel-list-img-top
				channel="video"
				:top="8"
				mode="16/9"
				to="/video/show/{0}"
				:skeRow="4"
				:show="{title:true, tag:true, like:true, msg:true}" />
		</div>
	</section>
	<!--图片分享-->
	<section class="bg-white">
		<div class="wrapper">
			<div class="title-box">
				<h3>图片分享</h3>
				<p class="text">关注行业动态，跟进时代发展，不断创新</p>
			</div>
			<channel-list-img-top
				channel="photo"
				:top="8"
				mode="4/3"
				to="/photo/show/{0}"
				:skeRow="4"
				:show="{title:true, tag:true, like:true, msg:true}" />
		</div>
	</section>
	<!--资源下载-->
	<section>
		<div class="wrapper">
			<div class="title-box">
				<h3>资源下载</h3>
				<p class="text">关注行业动态，跟进时代发展，不断创新</p>
			</div>
			<channel-list-img-top
				channel="down"
				:top="8"
				mode="4/3"
				to="/down/show/{0}"
				:skeRow="4"
				:show="{title:true, info:true, date:true, click:true}" />
		</div>
	</section>
</template>

<script setup>
	//使用首页布局
	definePageMeta({ layout: 'home' })
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: siteConfig.seoTitle,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
</script>