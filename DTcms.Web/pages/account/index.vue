<template>
	<NuxtLayout name="account">
		<template #location>
			<el-breadcrumb :separator-icon="ElIconArrowRight">
				<el-breadcrumb-item>当前所在位置</el-breadcrumb-item>
				<el-breadcrumb-item>会员中心</el-breadcrumb-item>
				<el-breadcrumb-item>管理首页</el-breadcrumb-item>
			</el-breadcrumb>
		</template>
		<el-row :gutter="20">
			<el-col :span="12">
				<account-balance-total-box title="账户余额" />
			</el-col>
			<el-col :span="12">
				<account-point-total-box title="账户积分" />
			</el-col>
		</el-row>
		
		<div class="bar-title mart-20">
			<div class="title">待处理投搞</div>
		</div>
		<account-contribute-list-top :top="4" :status="2" imgWidth="20%" to="/account/contribute/edit/{0}" />
		
	</NuxtLayout>
</template>

<script setup>
	//路由守卫标识
	const metas = definePageMeta({
		auth: true
	})
</script>