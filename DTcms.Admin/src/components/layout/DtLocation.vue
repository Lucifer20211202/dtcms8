<template>
	<div class="location">
		<el-breadcrumb separator-icon="arrow-right">
			<el-breadcrumb-item>
				<i class="iconfont icon-home"></i>
				管理中心
			</el-breadcrumb-item>
			<el-breadcrumb-item v-for="(item,index) in props.data" :key="index" :to="item.url?item.url:null">
				{{item.title}}
			</el-breadcrumb-item>
		</el-breadcrumb>
	</div>
</template>

<script setup>
	import { ref } from "vue"
	
	//接收props传值
	const props = defineProps({
		data: {
			type: Array,
			default: []
		}
	})
</script>

<style lang="scss">
	/*面包屑导航*/
	.location {
		margin: 0 20px 20px 20px;
		padding: 20px 0 5px;
		.el-breadcrumb {
			font-size: 13px;
			white-space: nowrap;
			text-overflow: ellipsis;
			overflow: hidden;
			.el-breadcrumb__item {
				float: none;
				.el-icon-arrow-right {
					margin: 0 0 0 3px;
				}
			}
		}
	}
</style>