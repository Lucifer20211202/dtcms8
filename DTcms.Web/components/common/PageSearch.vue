<template>
	<div class="search-box">
		<el-input v-model="datas.keyword" placeholder="请输入查询关健字" @keyup.enter="doSearch" clearable>
			<template #prepend>
				<el-select v-model="datas.channel" placeholder="请选择..">
					<el-option v-for="(item,index) in datas.data"
						:key="index" :label="item.title" :value="item.value" />
				</el-select>
			</template>
			<template #append>
				<el-button :icon="ElIconSearch" @click="doSearch" />
			</template>
		</el-input>
	</div>
</template>

<script setup>
	//通知父组件响应
	const emits = defineEmits(['close'])
	//声明变量
	const datas = reactive({
		data:[
			{title: '资讯', value: '/news/list'},
			{title: '视频', value: '/video/list'},
			{title: '图片', value: '/photo/list'},
			{title: '下载', value: '/down/list'},
		],
		channel: null,
		keyword: null
	})
	
	//提交查询
	const doSearch = async() => {
		if(!datas.channel) {
			ElMessage.warning("请选择查询的频道")
			return
		}
		if(!datas.keyword) {
			ElMessage.warning("请输入要查询的关健字")
			return
		}
		//跳转到页面
		await navigateTo({
			path: datas.channel,
			query: {
				keyword: encodeURI(datas.keyword)
			}
		},
		{
			replace: true,
		})
		//通知回调
		emits('close')
	}
</script>

<style lang="scss">
	.search-box {
		padding: 0 0 20px 0;
		.el-select {
			width: 6rem;
		}
	}
</style>