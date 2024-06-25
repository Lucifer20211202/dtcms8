<template>
	<div class="login-wrap">
		<common-page-register title="注册" />
	</div>
</template>

<script setup>
	//不使用布局
	definePageMeta({ layout: false })
	
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `用户注册 - ${siteConfig.seoTitle}`,
		description: siteConfig.seoDescription,
	})
</script>