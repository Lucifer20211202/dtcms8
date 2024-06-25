<template>
	<div class="login-wrap">
		<common-page-login title="登录" />
	</div>
</template>

<script setup>
	//不使用布局
	definePageMeta({ layout: false })
	
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `用户登录 - ${siteConfig.seoTitle}`,
		description: siteConfig.seoDescription,
	})
</script>