<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统功能'},{title:'后台菜单'}]"></dt-location>
		<dt-toolbar-box>
			<template #default>
				<div class="list-box">
					<div class="l-list">
						<el-button-group>
							<el-button icon="Plus" @click="$common.linkUrl('/manager/menu/edit')">新增</el-button>
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
				<el-table ref="tableRef" v-loading="datas.loading" :data="datas.listData" row-key="id" default-expand-all stripe
					:tree-props="{children: 'children', hasChildren: 'hasChildren'}" class="table-list" @selection-change="handleSelectionChange">
					<el-table-column type="selection" width="45"></el-table-column>
					<el-table-column label="标题" min-width="160">
						<template #default="scope">
							<span class="grey">{{scope.row.id}}.</span>
							<span>{{scope.row.title}}</span>
						</template>
					</el-table-column>
					<el-table-column prop="name" label="标识" min-width="120">
						<template #default="scope">
							<span class="nowrap">{{scope.row.name}}</span>
						</template>
					</el-table-column>
					<el-table-column prop="linkUrl" label="链接" min-width="150">
						<template #default="scope">
							<span class="nowrap">{{scope.row.linkUrl}}</span>
						</template>
					</el-table-column>
					<el-table-column label="排序" width="120">
						<template #default="scope">
							<el-input-number size="small" v-model="scope.row.sortId"
								@change="updateField(scope.row.id, '/sortId', scope.row.sortId)" :min="-99999999" :max="99999999"></el-input-number>
						</template>
					</el-table-column>
					<el-table-column label="显示" width="80" align="center">
						<template #default="scope">
							<el-switch :active-value="0" :inactive-value="1" v-model="scope.row.status"
								@change="updateField(scope.row.id, '/status', scope.row.status)"></el-switch>
						</template>
					</el-table-column>
					<el-table-column fixed="right" label="操作" width="120">
						<template #default="scope">
							<el-button size="small" icon="Edit" @click="$common.linkUrl(`/manager/menu/edit/${scope.row.id}`)"></el-button>
							<el-button size="small" icon="Plus" @click="$common.linkUrl('/manager/menu/edit', {parentId:scope.row.id})"></el-button>
							<el-button size="small" type="danger" icon="Delete" @click="deleteItem(scope.row.id)"></el-button>
						</template>
					</el-table-column>
				</el-table>
			</el-card>
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
		keyword: '',
		listData: [],
		multipleSelection: [],
	})
	
	//初始化数据
	const initData = async() => {
		let sendUrl = '/admin/manager/menu'
		if (datas.keyword.length > 0) {
			sendUrl += `?keyword=${encodeURI(datas.keyword)}`
		}
		//获取列表
		await proxy.$api.request({
			url: sendUrl,
			beforeSend() {
				datas.loading = true
			},
			success(res) {
				datas.listData = res.data
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
			return false
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
				url: `/admin/manager/menu?ids=${ids.toString()}`,
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
				url: `/admin/manager/menu/${val}`,
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
			url: `/admin/manager/menu/${id}`,
			data: [{ "op": "replace", "path": path, "value": val }],
			success(res) { }
		})
	}
	//选中第几行
	const handleSelectionChange = (val) => {
		datas.multipleSelection = val
	}
	
	//执行初始化方法
	initData()
</script>