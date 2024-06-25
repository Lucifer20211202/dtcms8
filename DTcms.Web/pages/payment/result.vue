<template>
	<div class="wrapper padb-30">
		<div class="location">
			<el-breadcrumb :separator-icon="ElIconArrowRight">
				<el-breadcrumb-item>当前所在位置</el-breadcrumb-item>
				<el-breadcrumb-item>支付中心</el-breadcrumb-item>
				<el-breadcrumb-item>支付结果</el-breadcrumb-item>
			</el-breadcrumb>
		</div>
		<payment-result-page />
	</div>
</template>

<script setup>
	//路由守卫标识
	const metas = definePageMeta({
		auth: true
	})
</script>