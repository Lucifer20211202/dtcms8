<template>
	<el-affix :offset="50">
		<div ref="toolbar" class="toolbar-box">
			<slot name="default"></slot>
			<div v-if="slotMore" class="more-wrap">
				<el-icon class="more-btn" @click="change">
					<ArrowUp v-if="datas.more" />
					<ArrowDown v-else />
				</el-icon>
				<div v-show="datas.more">
					<slot name="more"></slot>
				</div>
			</div>
		</div>
	</el-affix>
</template>

<script setup>
	import { ref,reactive,useSlots,onMounted } from 'vue'
	
	const toolbar = ref(null)
	//判断Slot是否填充
	const slotMore = !!useSlots().more
	
	//定义属性
	const datas = reactive({
		more: true
	})
	
	//切换显示更多条件
	const change = () => {
		datas.more = !datas.more
	}
	
	//页面加载完成事件
	/*onMounted(() => {
		const resizeObserver = new ResizeObserver(entries => {
			entries.forEach(entry => {
				//console.log('DIV高度变化:', entry.target.offsetHeight)
			})
		})
		resizeObserver.observe(toolbar.value)
	})*/
</script>