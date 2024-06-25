<template>
	<div class="dropdown">
		<el-dropdown ref="drop" :trigger="trigger" placement="bottom-start">
			<el-button class="select" plain>
				<span>{{datas.title}}</span>
				<el-icon class="el-icon--right"><ArrowDown /></el-icon>
			</el-button>
			<template #dropdown>
				<el-dropdown-menu class="dropdown-menu">
					<el-tree ref="tree" :data="props.data" node-key="id" default-expand-all :current-node-key="props.modelValue" :highlight-current="true"
						:expand-on-click-node="false" @node-click="handleClick">
						<template #default="{node,data}">
							<span>{{ data.title }}</span>
						</template>
					</el-tree>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance,watch,nextTick } from "vue"
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//定义ref
	const drop = ref(null)
	const tree = ref(null)
	const emits = defineEmits(['update:modelValue'])
	//接收props传值
	const props = defineProps({
		trigger: {
			type: String,
			default: 'click'
		},
		placeholder: {
			type: String,
			default: '请选择..'
		},
		modelValue: {
			type: Number,
			default: 0
		},
		disabled: {
			type: Number,
			default: 0
		},
		data: {
			type: Array,
			default: []
		}
	})
	//定义变量
	const datas = reactive({
		title: props.placeholder
	})
	
	//选中回调
	const handleClick = (item, node, tList) => {
		if (props.disabled == item.id) {
			proxy.$message({ type: 'error', message: '不能将自己设为父级' })
			tree.value.setCurrentKey(props.modelValue)
			return false
		}
		datas.title = item.title
		//给父组件传值
		emits('update:modelValue', item.id)
		//触发组件的点击事件关闭下拉框
		drop.value.handleClose()
	}
	//迭代遍历树
	const initData = (obj) => {
		obj.map((item) => {
			if (item.id == props.modelValue) {
				datas.title = item.title
				tree.value.setCurrentKey(item.id)
				return false
			}else if (item.children) {
				initData(item.children)
			}
		})
	}
	
	//监视data赋值的方法
	watch(() => props.data, (newVal, oldVal) => {
		if (props.modelValue > 0 && newVal.length > 0) {
			nextTick(() => {
				initData(newVal)
			});
		}
	})
	//监视modelValue赋值的方法
	watch(() => props.modelValue, (newVal, oldVal) => {
		if (oldVal == 0 && newVal > 0 && props.data.length > 0) {
			nextTick(() => {
				initData(props.data)
			});
		}
		if(newVal === 0) {
			datas.title = props.placeholder
		}
	})
</script>