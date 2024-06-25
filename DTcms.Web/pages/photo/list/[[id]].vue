<template>
	<div class="wrapper">
		<channel-page-list>
			<template #default="prop">
				<!--条件筛选-->
				<div class="mart-30 marb-30">
					<!--分类-->
					<channel-tabs-category v-model="prop.datas.categoryId" channel="photo" @change="initData" />
					<!--标签-->
					<channel-tabs-label v-model="prop.datas.labelId" :top="0" @change="initData" />
				</div>
				<!--列表-->
				<channel-list-img-page
					ref="divRef"
					channel="photo"
					:categoryId="prop.datas.categoryId"
					:labelId="prop.datas.labelId"
					:keyword="prop.datas.keyword"
					mode="4/3"
					to="/photo/show/{0}"
					:pageSize="10"
					:skeRow="4"
					:show="{title:true, tag:true, like:true, msg:true}" />
			</template>
		</channel-page-list>
	</div>
</template>

<script setup>
	//声明Ref
	const divRef = ref(null)
	//客户端加载分页数据
	const initData = async() => {
		//调用子组件方法
		divRef.value.initData()
	}
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>