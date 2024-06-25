<template>
	<el-col :span="12">
		<el-badge :value="datas.count" v-loading="datas.loading">
			<a @click="$common.linkUrl(props.link)">
				<el-icon><component :is="props.icon" /></el-icon>
				<span>{{props.title}}</span>
			</a>
		</el-badge>
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
		icon: {
			type: String,
			default: () => {
				return null
			}
		},
		uri: {
			type: String,
			default: () => {
				return null
			}
		},
		link: {
			type: String,
			default: () => {
				return ''
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