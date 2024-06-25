<template>
	<div class="tabs-nav-item">
		<div class="list">
			<div class="text" :class="{'active': props.modelValue == 0}" @click="handleSelected(0)">
				全部
			</div>
			<div class="text"
				v-for="(item,index) in datas.listData"
				:key="index"
				:class="{'active': props.modelValue == item.id}"
				@click="handleSelected(item.id)">
				{{item.title}}
			</div>
		</div>
	</div>
	<div class="tabs-nav-text" v-if="datas.itemData.children">
		<div class="title">{{datas.itemData.title}}</div>
		<div class="list">
			<div class="text"
				v-for="(item,index) in datas.itemData.children"
				:key="index"
				:class="{'active': props.modelValue == item.id}"
				@click="handleSelected(item.id)">
				{{item.title}}
			</div>
		</div>
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	
	//通知父组件响应
	const emits = defineEmits(['update:modelValue','change'])
	//接收props传值
	const props = defineProps({
		modelValue: {
			type: Number,
			default: 0,
		},
		//频道名称
		channel: {
			type: String,
			default: () => {
				return null
			}
		},
	})
	//声明变量
	const datas = reactive({
		listData: [],
		itemData: {},
	})
	
	//服务端获取
	let sendUrl = `/client/article/category/channel/${props.channel}?siteId=${siteConfig.id}`
	const {data: listRef} = await useAsyncData(sendUrl, () => useHttp({url: sendUrl}))
	datas.listData = listRef.value?.data
	
	//选择回调
	const handleSelected = (val) => {
		if(val > 0 && datas.listData) {
			datas.listData.forEach((item,index) => {
				if(item.id == val) {
					datas.itemData = item
					return
				}
				if(item.children.length > 0) {
					item.children.forEach((sitem,sindex) => {
						if(sitem.id === val) {
							datas.itemData = item
							return
						}
					})
				}
			})
		} else {
			datas.itemData = {}
		}
		//通知更新
		emits('update:modelValue', val)
		emits('change')
	}
	
	//页面完成后执行
	onMounted(() => {
		handleSelected(props.modelValue)
	})
</script>