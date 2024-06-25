<template>
	<div class="tabs-nav-text">
		<div class="title">筛选标签</div>
		<div class="list">
			<div class="text" :class="{'active': props.modelValue == 0}" @click="handleSelected(0)">全部</div>
			<div class="text"
				v-for="(item,index) in datas.listData"
				:key="index"
				:class="{'active': props.modelValue == item.id}"
				@click="handleSelected(item.id)">
				{{item.title}}
			</div>
		</div>
	</div>
</template>

<script setup>
	//通知父组件响应
	const emits = defineEmits(['update:modelValue','change'])
	//接收props传值
	const props = defineProps({
		modelValue: {
			type: Number,
			default: 0,
		},
		//显示条数
		top: {
			type: Number,
			default: 0
		},
	})
	//声明变量
	const datas = reactive({
		listData: [],
	})
	
	//服务端获取
	let sendUrl = `/client/article/label/view/${props.top}`
	const {data: listRef} = await useAsyncData(sendUrl, () => useHttp({url: sendUrl}))
	datas.listData = listRef.value?.data
	
	//选择回调
	const handleSelected = (val) => {
		//通知更新
		emits('update:modelValue', val)
		emits('change')
	}
</script>