<template>
	<el-col :span="24" class="mab-20">
		<el-card v-loading="datas.loading">
			<dt-chart-line :type="props.type" height="259px" :data="datas.listData" :node="{name:'title', value:'total'}"
				:title="{text:'会员注册情况',subtext:'默认显示当月统计数据',valtext:'新增用户'}" />
		</el-card>
	</el-col>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtChartLine from "../echart/DtChartLine.vue"
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//接收props传值
	const props = defineProps({
		type: {
			type: String,
			default: () => {
				return 'bar'
			}
		}
	})
	//定义组件属性
	const datas = reactive({
		loading: false,
		listData: []
	})
	
	//初始化数据
	const initData = () => {
		proxy.$api.request({
			url: '/admin/member/view/report',
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data
			},
			error(err) {
				datas.listData = []
			},
			complete() {
				datas.loading = false
			}
		})
	}
	
	initData()
</script>