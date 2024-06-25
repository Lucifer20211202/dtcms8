<template>
	<NuxtLayout name="account">
		<template #location>
			<el-breadcrumb :separator-icon="ElIconArrowRight">
				<el-breadcrumb-item>当前所在位置</el-breadcrumb-item>
				<el-breadcrumb-item>会员中心</el-breadcrumb-item>
				<el-breadcrumb-item>在线充值</el-breadcrumb-item>
			</el-breadcrumb>
		</template>
		<div class="radius-box full pad-20">
			<account-recharge-edit-page />
		</div>
	</NuxtLayout>
</template>

<script setup>
	//路由守卫标识
	const metas = definePageMeta({
		auth: true
	})
</script>