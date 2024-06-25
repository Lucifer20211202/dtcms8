<template>
	<div ref="divRef">
		<el-skeleton :rows="5" :loading="datas.loading" animated>
			<client-only>
				<el-form ref="formRef" :model="props.modelValue" :rules="props.rules" :label-position="datas.position" :label-width="props.labelWidth" class="form-box">
					<el-tabs v-model="datas.activeName" class="form-tabs">
						<slot name="default"></slot>
					</el-tabs>
				</el-form>
			</client-only>
		</el-skeleton>
	</div>
</template>

<script setup>
	import { ref,reactive,nextTick,onMounted } from "vue"
	//定义REF
	const formRef = ref(null)
	const divRef = ref(null)
	//接收props传值
	const props = defineProps({
		modelValue: {
			type: Object,
			default: null,
		},
		rules: {
			type: Object,
			default: null,
		},
		labelWidth: {
			type: String,
			default: '120px',
		},
		activeName: {
			type: String,
			default: null,
		}
	})
	//定义组件属性
	const datas = reactive({
		loading: true,
		activeName: props.activeName,
		position: 'right'
	})
	
	//暴露组件方法
	defineExpose({
		form: formRef
	})
	
	//页面加载完成事件
	onMounted(() => {
		datas.loading = false
		nextTick(() => {
			const resizeObserver = new ResizeObserver(entries => {
				entries.forEach(entry => {
					//console.log('DIV宽度变化:', entry.target.offsetWidth)
					if(entry.target.offsetWidth < 400) {
						datas.position = 'top'
					} else {
						datas.position = 'right'
					}
				})
			})
			resizeObserver.observe(divRef.value)
		})
	})
	
</script>