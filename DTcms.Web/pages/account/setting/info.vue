<template>
	<NuxtLayout name="account">
		<template #location>
			<el-breadcrumb :separator-icon="ElIconArrowRight">
				<el-breadcrumb-item>当前所在位置</el-breadcrumb-item>
				<el-breadcrumb-item>会员中心</el-breadcrumb-item>
				<el-breadcrumb-item>个人资料</el-breadcrumb-item>
			</el-breadcrumb>
		</template>
		<div class="radius-box full pad-20">
			<account-setting-info-page />
		</div>
	</NuxtLayout>
</template>

<script setup>
	//路由守卫标识
	const metas = definePageMeta({
		auth: true
	})
</script>