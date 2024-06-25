<template>
	<el-col :span="12">
		<el-card v-loading="datas.loading">
			<h1>{{datas.count}}<span v-if="props.unit">{{props.unit}}</span></h1>
			<p v-if="props.title">{{props.title}}</p>
		</el-card>
	</el-col>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//接收props传值
	const props = defineProps({
		title: {
			type: String,
			default: () => {
				return null
			}
		},
		unit: {
			type: String,
			default: () => {
				return '个'
			}
		},
		uri: {
			type: String,
			default: () => {
				return null
			}
		}
	})
	//定义组件属性
	const datas = reactive({
		loading: false,
		count: 0
	})
	
	//初始化数据
	const initData = () => {
		if(!props.uri) return false
		proxy.$api.request({
			url: props.uri,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.count = res.data
			},
			complete() {
				datas.loading = false
			}
		})
	}
	
	initData()
</script>