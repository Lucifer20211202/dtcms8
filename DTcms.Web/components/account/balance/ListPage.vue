<template>
	<div class="tabel-search marb-20">
		<div class="tips">
			<el-icon><ElIconOpportunity /></el-icon>
			<span>您当前账户的余额：{{userInfo.amount}} 元</span>
		</div>
		<div class="left">
			<el-button type="primary" :icon="ElIconPlus" @click="common.linkUrl('/account/recharge/add')">充值</el-button>
		</div>
	</div>
	<div class="radius-box hidden">
		<el-skeleton :loading="datas.loading" animated>
			<template #template>
				<div class="pad-20">
					<el-skeleton-item variant="h1" style="width:30%" />
					<el-skeleton-item variant="p" />
					<el-skeleton-item variant="p" />
					<el-skeleton-item variant="p" />
					<el-skeleton-item variant="p" style="width:80%" />
				</div>
			</template>
			<el-table ref="tableRef" v-loading="datas.loading" :data="datas.listData" stripe class="table-list">
				<el-table-column prop="description" label="备注说明" min-width="120"></el-table-column>
				<el-table-column prop="currAmount" label="当时余额" width="120"></el-table-column>
				<el-table-column label="交易金额" width="120">
					<template #default="scope">
						<span class="price" v-if="scope.row.value>0">+{{scope.row.value}}</span>
						<span class="price" v-else>{{scope.row.value}}</span>
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="交易时间" width="170">
					<template #default="scope">
						{{scope.row.addTime}}
					</template>
				</el-table-column>
			</el-table>
		</el-skeleton>
	</div>
	<div class="pager-box">
		<el-pagination background
			:page-size="datas.pageSize"
			:current-page="datas.pageIndex"
			:total="datas.totalCount"
			layout="prev,pager,next"
			@current-change="handleCurrentChange" />
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `账户余额 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	//获取当前用户信息
	const userInfo = await useUser('info')
	
	//定义Ref变量
	const tableRef = ref(null)
	//定义属性
	const datas = reactive({
		loading: true,
		keyword: '',
		totalCount: 0,
		pageSize: 10,
		pageIndex: 1,
		listData: [],
		multipleSelection: [],
	})
	
	//初始化数据
	const initData = async() => {
		let sendUrl = `/account/member/balance?pageSize=${datas.pageSize}&pageIndex=${datas.pageIndex}`
		if (datas.keyword.length > 0) {
			sendUrl += `&keyword=${encodeURI(datas.keyword)}`
		}
		//获取分页列表
		useHttp({
			url: sendUrl,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data
				datas.totalCount = res.pagination.totalCount
				datas.pageIndex = res.pagination.pageIndex
				datas.pageSize = res.pagination.pageSize
			},
			error(err) {
				datas.listData = []
			},
			complete() {
				datas.loading = false
			}
		})
	}
	//跳转到第几页
	const handleCurrentChange = (val) => {
		if (datas.pageIndex != val) {
			datas.pageIndex = val
			initData()
		}
	}
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>