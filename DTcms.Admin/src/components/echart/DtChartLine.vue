<template>
	<div ref="chartRef" class="chart-box" :style="`width:${props.width};height:${props.height};`"></div>
</template>

<script setup>
	import { ref,reactive,nextTick,onMounted,watch } from "vue"
	import * as echarts from 'echarts'
	
	//定义REF
	const chartRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		//图标类型
		type: {
			type: String,
			default: 'line'
		},
		//源数据
		data: {
			type: Array,
			default: []
		},
		//节点名称
		node: {
			name: null,
			value: null
		},
		//显示标题
		title: {
			text: '',
			subtext: '',
			valtext: ''
		},
		//容器宽度
		width: {
			type: String,
			default: '100%'
		},
		//容器高度
		height: {
			type: String,
			default: '320px'
		}
	})
	//定义 Chart 实例
	let chartInstance = null
	
	//初始化图表
	const initChart = () => {
		if(!chartInstance) return false
		
		//处理数据
		let titles = []
		let values = []
		props.data.forEach((item, index) => {
			if(props.node.name) {
				titles.push(item[props.node.name])
			}
			if(props.node.value) {
				values.push(item[props.node.value])
			}
		})
		//绘制图表
		chartInstance.setOption({
			color: ['#3398DB'],
			title: { text: props.title.text, subtext: props.title.subtext, textStyle: { fontSize: 16, fontWeight: "normal" } },
			tooltip: {
				trigger: 'axis',
				axisPointer: {
					type: 'shadow'
				}
			},
			grid: {
				left: '2%',
				right: '2%',
				bottom: '5%',
				width: 'auto',
				height: 'auto',
				containLabel: true
			},
			xAxis: {
				data: titles
			},
			yAxis: {
				type: 'value',
				minInterval: 1
			},
			series: [{
				name: props.title.valtext,
				type: props.type,
				data: values
			}]
		})
	}
	
	//页面加载完成事件
	onMounted(async() => {
		await nextTick()
		//创建 chart实例
		chartInstance = echarts.init(chartRef.value)
		//初始化 chart实例
		initChart()
		//监听页面宽度
		const resizeObserver = new ResizeObserver(entries => {
			entries.forEach(entry => {
				//console.log('DIV宽度变化:', entry.target.offsetWidth)
				chartInstance.resize()
			})
		})
		resizeObserver.observe(chartRef.value)
	})
	//监听 data 的变化，由于是数组需要深度监听
	watch(() => props.data, (newVal, oldVal) => {
		if (newVal.length) {
			initChart()
		}
	},{ immediate: true, deep: true })
</script>