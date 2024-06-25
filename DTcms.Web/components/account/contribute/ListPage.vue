<template>
	<div class="tabel-search marb-20">
		<div class="left">
			<el-button type="primary" :icon="ElIconPlus" @click="common.linkUrl('/account/contribute/edit')">添加</el-button>
			<el-button type="danger" :icon="ElIconDelete" @click="deleteCheckAll">删除</el-button>
		</div>
		<div class="right">
			<el-input v-model="datas.keyword" placeholder="请输入关健字" @clear="initData" @keyup.enter.native="initData" clearable>
				<template #append>
					<el-button :icon="ElIconSearch" @click="initData" />
				</template>
			</el-input>
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
				<el-table-column label="标题" min-width="220">
					<template #default="scope">
						<el-image class="pic" fit="contain" :src="common.loadFile(scope.row.imgUrl)" :preview-src-list="[common.loadFile(scope.row.imgUrl)]" preview-teleported>
							<template #error>
								<div class="image-slot">
									<el-icon><ElIconPicture /></el-icon>
								</div>
							</template>
						</el-image>
						<h4>{{scope.row.title}}</h4>
						<p>{{scope.row.zhaiyao}}</p>
						<span class="date">
							<el-icon><ElIconCalendar /></el-icon>
							{{scope.row.addTime}}
						</span>
					</template>
				</el-table-column>
				<el-table-column label="状态" width="90" align="center">
					<template #default="scope">
						<el-tag effect="dark" type="warning" v-if="scope.row.status==0">待审</el-tag>
						<el-tag effect="dark" type="success" v-else-if="scope.row.status==1">通过</el-tag>
						<el-tag effect="dark" type="danger" v-else="scope.row.status==2">驳回</el-tag>
					</template>
				</el-table-column>
				<el-table-column fixed="right" label="操作" width="98">
					<template #default="scope">
						<el-button size="small" :icon="ElIconEdit" @click="common.linkUrl(`/account/contribute/edit/${scope.row.id}`)"></el-button>
						<el-button size="small" type="danger" :icon="ElIconDelete" @click="deleteItem(scope.row.id)"></el-button>
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
		title: `投稿管理 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	
	//定义Ref变量
	const tableRef = ref(null)
	//定义属性
	const datas = reactive({
		loading: false,
		keyword: '',
		totalCount: 0,
		pageSize: 10,
		pageIndex: 1,
		listData: [],
		multipleSelection: [],
	})
	
	//初始化数据
	const initData = async() => {
		let sendUrl = `/account/article/contribute?pageSize=${datas.pageSize}&pageIndex=${datas.pageIndex}`
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
				url: `/account/article/contribute?ids=${ids.toString()}`,
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
				url: `/account/article/contribute/${val}`,
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