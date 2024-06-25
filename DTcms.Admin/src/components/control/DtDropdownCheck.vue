<template>
	<div class="dropdown">
		<el-dropdown :trigger="props.trigger" placement="bottom-start">
			<el-button class="check" icon="Plus"></el-button>
			<template #dropdown>
				<el-dropdown-menu class="dropdown-menu">
					<el-tree ref="tree" :data="props.data" node-key="id" default-expand-all show-checkbox check-on-click-node 
						:check-strictly="props.checkStrictly" 
						:expand-on-click-node="false"
						@check="handleCheck">
						<template #default="{node,data}">
							<span>{{ data.title }}</span>
						</template>
					</el-tree>
				</el-dropdown-menu>
			</template>
		</el-dropdown>
		<el-tag size="large" effect="dark" :key="item.id" v-for="item in props.modelValue" disable-transitions closable 
			@close="handleClose(item)">
			{{item.title}}
		</el-tag>
	</div>
</template>

<script setup>
	import { ref,watch,nextTick } from "vue"
	
	//定义ref
	const tree = ref(null)
	const emits = defineEmits(['update:modelValue'])
	//接收props传值
	const props = defineProps({
		trigger: {
			type: String,
			default: 'click'
		},
		checkStrictly: {
			type: Boolean,
			default: true
		},
		checked: {
			type: Array,
			default: []
		},
		modelValue: {
			type: Array,
			default: []
		},
		data: {
			type: Array,
			default: []
		}
	})
	
	//删除Tag的回调
	const handleClose = (val) => {
		//删除当前选中项
		props.modelValue.forEach((item, i) => {
			if (item == val) {
				props.modelValue.splice(i, 1);
			}
		})
		//重新设置选中值
		let nodeKeys = [];
		props.modelValue.forEach((item, i) => {
			nodeKeys.push(item.id);
		})
		tree.value.setCheckedKeys(nodeKeys);
		emits('update:modelValue', props.modelValue)
	}
	//选中发生变化回调
	const handleCheck = () => {
		let nodes = []; //临时节点
		tree.value.getCheckedNodes().map(item => {
			nodes.push(item);
		});
		emits('update:modelValue', nodes);
	}
	
	//监视data赋值的方法
	watch(() => props.data, (newVal, oldVal) => {
		if (newVal.length>0) {
			nextTick(() => {
				tree.value.setCheckedKeys(props.checked)
				handleCheck()
			})
		}
	})
	//监视checked赋值的方法
	watch(() => props.checked, (newVal, oldVal) => {
		if (newVal.length>0) {
			nextTick(() => {
				tree.value.setCheckedKeys(newVal)
				handleCheck()
			})
		}
	})
</script>