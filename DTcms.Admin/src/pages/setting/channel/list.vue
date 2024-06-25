<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统管理'},{title:'频道列表'}]"></dt-location>
		<dt-toolbar-box>
			<template #default>
				<div class="list-box">
					<div class="l-list">
						<el-button-group>
							<el-button icon="Plus" @click="$common.linkUrl('/setting/channel/edit')">新增</el-button>
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
			<template #more>
				<div class="more-box">
					<dl>
						<dt>筛选站点</dt>
						<dd>
							<el-select v-model="datas.siteId" @change="initData" placeholder="请选择...">
								<el-option key="0" label="所有站点" :value="0"></el-option>
								<el-option v-for="item in datas.siteList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</dd>
					</dl>
				</div>
			</template>
		</dt-toolbar-box>
		
		<div class="content-box">
			<el-card class="table-card">
				<el-table ref="tableRef" v-loading="datas.loading" :data="datas.listData" stripe class="table-list" @selection-change="handleSelectionChange">
					<el-table-column type="selection" width="45"></el-table-column>
					<el-table-column prop="id" label="序号" width="80"></el-table-column>
					<el-table-column prop="name" label="名称" min-width="100"></el-table-column>
					<el-table-column prop="title" label="标题" min-width="180"></el-table-column>
					<el-table-column prop="isComment" label="评论" width="60" align="center">
						<template #default="scope">
							<el-icon v-if="scope.row.isComment===1"><Check /></el-icon>
							<el-icon v-else><Close /></el-icon>
						</template>
					</el-table-column>
					<el-table-column prop="isAlbum" label="相册" width="60" align="center">
						<template #default="scope">
							<el-icon v-if="scope.row.isAlbum===1"><Check /></el-icon>
							<el-icon v-else><Close /></el-icon>
						</template>
					</el-table-column>
					<el-table-column prop="isAttach" label="附件" width="60" align="center">
						<template #default="scope">
							<el-icon v-if="scope.row.isAttach===1"><Check /></el-icon>
							<el-icon v-else><Close /></el-icon>
						</template>
					</el-table-column>
					<el-table-column prop="isContribute" label="投稿" width="60" align="center">
						<template #default="scope">
							<el-icon v-if="scope.row.isContribute===1"><Check /></el-icon>
							<el-icon v-else><Close /></el-icon>
						</template>
					</el-table-column>
					<el-table-column label="排序" width="120">
						<template #default="scope">
							<el-input-number size="small" v-model="scope.row.sortId"
								@change="updateField(scope.row.id, '/sortId', scope.row.sortId)" :min="-99999999" :max="99999999"></el-input-number>
						</template>
					</el-table-column>
					<el-table-column label="启用" width="90" align="center">
						<template #default="scope">
							<el-switch :active-value="0" :inactive-value="1" v-model="scope.row.status" @change="editStatus(scope.row.id, scope.row.status)"></el-switch>
						</template>
					</el-table-column>
					<el-table-column fixed="right" label="操作" width="90">
						<template #default="scope">
							<el-button size="small" icon="Edit" @click="$common.linkUrl(`/setting/channel/edit/${scope.row.id}`)"></el-button>
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
	//通知父组件响应
	const emit = defineEmits(['loadMenu'])
	
	//定义属性
	const datas = reactive({
		loading: false,
		siteId: 0, //所属站点
		keyword: '',
		totalCount: 0,
		pageSize: 10,
		pageIndex: 1,
		siteList: [],
		listData: [],
		multipleSelection: [],
	})
	
	//加载数据
	const loadData = async() => {
		await proxy.$api.request({
			url: '/admin/site/view/0',
			success(res) {
				datas.siteList = res.data
			}
		})
		//初始化数据
		await initData()
	}
	//初始化数据
	const initData = async() => {
		let sendUrl = `/admin/channel?pageSize=${datas.pageSize}&pageIndex=${datas.pageIndex}`
		if (datas.siteId) {
			sendUrl += `&siteId=${datas.siteId}` 
		}
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
		let list = datas.multipleSelection
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
				url: `/admin/channel?ids=${ids.toString()}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					tableRef.value.clearSelection() //清除选中状态
					initData() //重新加载列表
					emit('loadMenu') //通知父组件重新加载菜单
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
				url: `/admin/channel/${val}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					initData() //重新加载列表
					emit('loadMenu') //通知父组件重新加载菜单
				}
			})
			
		}).catch(() => { })
	}
	//修改部分字段
	const updateField = (id, path, val) => {
		proxy.$api.request({
			method: 'patch',
			url: `/admin/channel/${id}`,
			data: [{ "op": "replace", "path": path, "value": val }],
			success(res) { }
		})
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
	loadData()
</script>