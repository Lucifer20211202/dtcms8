<template>
	<NuxtLayout name="account">
		<template #location>
			<el-breadcrumb :separator-icon="ElIconArrowRight">
				<el-breadcrumb-item>当前所在位置</el-breadcrumb-item>
				<el-breadcrumb-item>会员中心</el-breadcrumb-item>
				<el-breadcrumb-item>账户积分</el-breadcrumb-item>
			</el-breadcrumb>
		</template>
		<account-point-list-page />
	</NuxtLayout>
</template>

<script setup>
	//路由守卫标识
	const metas = definePageMeta({
		auth: true
	})
</script>