<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员明细'},{title:'积分记录'}]"></dt-location>
		<dt-toolbar-box>
			<template #default>
				<div class="list-box">
					<div class="l-list">
						<el-button-group>
							<el-button icon="Plus" @click="$common.linkUrl('/member/point/edit')">新增</el-button>
							<el-button icon="Delete" @click="deleteCheckAll">删除</el-button>
						</el-button-group>
					</div>
					<div class="r-list">
						<div class="search-box">
							<el-input placeholder="输入关健字" v-model="datas.keyword" @clear="initData" @keyup.enter.native="initData" clearable>
								<template #append>
									<el-button icon="Search" @click="initData"></el-button>
								</template>
							</el-input>
						</div>
					</div>
				</div>
			</template>
		</dt-toolbar-box>
		
		<div class="content-box">
			<el-card class="table-card">
				<el-table ref="tableRef" v-loading="datas.loading" :data="datas.listData" stripe class="table-list" @selection-change="handleSelectionChange">
					<el-table-column type="selection" width="45"></el-table-column>
					<el-table-column prop="userName" label="用户名" width="120"></el-table-column>
					<el-table-column prop="currPoint" label="账户积分" width="110"></el-table-column>
					<el-table-column prop="value" label="操作积分" width="110"></el-table-column>
					<el-table-column prop="description" label="备注" min-width="120"></el-table-column>
					<el-table-column prop="addTime" label="记录时间" width="160"></el-table-column>
					<el-table-column fixed="right" label="操作" width="90">
						<template #default="scope">
							<el-button size="small" icon="View" @click="$common.linkUrl(`/member/point/edit/${scope.row.id}`)"></el-button>
							<el-button size="small" type="danger" icon="Delete" @click="deleteItem(scope.row.id)"></el-button>
						</template>
					</el-table-column>
				</el-table>
			</el-card>
			
			<div class="pager-box">
				<el-pagination background
					@size-change="handleSizeChange"
					@current-change="handleCurrentChange"
					:current-page="datas.pageIndex"
					:page-sizes="[10, 20, 50, 100]"
					:page-size="datas.pageSize"
					layout="total, sizes, prev, pager, next, jumper"
					:total="datas.totalCount">
				</el-pagination>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtToolbarBox from '../../../components/layout/DtToolbarBox.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
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
		let sendUrl = `/admin/member/point?pageSize=${datas.pageSize}&pageIndex=${datas.pageIndex}`
		if (datas.keyword.length > 0) {
			sendUrl += `&keyword=${encodeURI(datas.keyword)}`
		}
		//获取分页列表
		await proxy.$api.request({
			url: sendUrl,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data;
				let pageInfo = JSON.parse(res.headers["x-pagination"])
				datas.totalCount = pageInfo.totalCount
				datas.pageSize = pageInfo.pageSize
				datas.pageIndex = pageInfo.pageIndex
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
		let list = datas.multipleSelection;
		//检查是否有选中
		if (!list.length) {
			proxy.$message({ type: 'warning', message: '请选择要删除的记录' })
			return false;
		}
		//执行删除操作
		proxy.$confirm('确认要删除该记录吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			let ids = list.map(obj => obj.id)
			proxy.$api.request({
				method: 'delete',
				url: `/admin/member/point?ids=${ids.toString()}`,
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
		proxy.$confirm('确认要删除该记录吗？', '提示', {
			confirmButtonText: '确定',
			cancelButtonText: '取消',
			type: 'warning'
		}).then(() => {
			proxy.$api.request({
				method: 'delete',
				url: `/admin/member/point/${val}`,
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
	//每页显示数量
	const handleSizeChange = (val) => {
		if (datas.pageSize != val) {
			datas.pageSize = val
			initData()
		}
	}
	//跳转到第几页
	const handleCurrentChange = (val) => {
		if (datas.pageIndex != val) {
			datas.pageIndex = val
			initData()
		}
	}
	
	//执行初始化方法
	initData()
</script>