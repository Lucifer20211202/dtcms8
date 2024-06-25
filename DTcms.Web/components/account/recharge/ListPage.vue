<template>
	<div class="tabel-search marb-20">
		<div class="tips">
			<el-icon><ElIconOpportunity /></el-icon>
			<span>您当前账户的余额：{{userInfo.amount}} 元</span>
		</div>
		<div class="left">
			<el-button type="primary" :icon="ElIconPlus" @click="common.linkUrl('/account/recharge/add')">充值</el-button>
			<el-button type="danger" :icon="ElIconDelete" @click="deleteCheckAll">删除</el-button>
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
			<el-table ref="tableRef" v-loading="datas.loading" :data="datas.listData" stripe class="table-list" @selection-change="handleSelectionChange">
				<el-table-column type="selection" width="45"></el-table-column>
				<el-table-column prop="tradeNo" label="交易号" min-width="120"></el-table-column>
				<el-table-column prop="paymentTitle" label="支付方式" width="120"></el-table-column>
				<el-table-column prop="amount" label="交易金额" width="120"></el-table-column>
				<el-table-column prop="addTime" label="交易时间" width="168"></el-table-column>
				<el-table-column label="交易状态" width="120">
					<template #default="scope">
						<el-tag v-if="scope.row.status===1" type="success">已完成</el-tag>
						<el-tag v-else-if="scope.row.status===2" type="info">已取消</el-tag>
						<el-tag v-else type="warning">待付款</el-tag>
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="操作" width="120">
					<template #default="scope">
						<el-button size="small" :disabled="scope.row.status>0" @click="common.linkUrl('/payment/confirm',{no: scope.row.tradeNo})">去付款</el-button>
						<el-button size="small" type="danger" :icon="ElIconDelete" @click="deleteItem(scope.row.id)" :disabled="scope.row.status===1"></el-button>
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
		title: `充值记录 - ${siteConfig.seoKeyword}`,
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
		let sendUrl = `/account/member/recharge?pageSize=${datas.pageSize}&pageIndex=${datas.pageIndex}`
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
	//多选删除
	const deleteCheckAll = () => {
		//拿到选中的数据
		let list = datas.multipleSelection
		//检查是否有选中
		if (!list.length) {
			ElMessage.warning('请选择要删除的记录')
			return false
		}
		//执行删除操作
		ElMessageBox.confirm('确认要删除该记录吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			let ids = list.map(obj => obj.id)
			useHttp({
				method: 'delete',
				url: `/account/member/recharge?ids=${ids.toString()}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					tableRef.value.clearSelection() //清除选中状态
					initData() //重新加载列表
				}
			})
		}).catch(() => { })
	}
	//单项删除
	const deleteItem = (val) => {
		//执行删除操作
		ElMessageBox.confirm('确认要删除该记录吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			useHttp({
				method: 'delete',
				url: `/account/member/recharge/${val}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					initData() //重新加载列表
				}
			})
			
		}).catch(() => { })
	}
	//选中第几行
	const handleSelectionChange = (val) => {
		datas.multipleSelection = val
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