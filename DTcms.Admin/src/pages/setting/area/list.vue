<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统功能'},{title:'地区管理'}]"></dt-location>
		<dt-toolbar-box>
			<template #default>
				<div class="list-box">
					<div class="l-list">
						<el-button-group>
							<el-button icon="Plus" @click="$common.linkUrl('/setting/area/edit')">新增</el-button>
							<el-button icon="CopyDocument" @click="datas.showImportDialog=true">导入</el-button>
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
			<el-card class="tree-card">
				<el-tree ref="treeListRef" :data="datas.listData" v-loading="datas.loading" node-key="id" show-checkbox check-on-click-node :expand-on-click-node="false">
					<template #default="{ node, data }">
						<div class="tree-node">
							<span>{{ data.title }}</span>
							<div class="tools-box" style="margin-left: 0;">
								<el-input-number size="small" v-model="data.sortId"
									@change="updateField(scope.row.id, '/sortId', scope.row.sortId)"></el-input-number>
								<el-button size="small" icon="Edit" @click="$common.linkUrl(`/setting/area/edit/${data.id}`)"></el-button>
								<el-button size="small" icon="Plus" @click="$common.linkUrl('/setting/area/edit', {parentId:data.id})"></el-button>
								<el-button size="small" type="danger" icon="Delete" @click="deleteItem(data.id)"></el-button>
							</div>
						</div>
					</template>
				</el-tree>
			</el-card>
			
			<el-dialog v-model="datas.showImportDialog" title="批量导入" fullscreen>
				<dt-form-box ref="importFormRef" v-model="datas.importModel" :rules="datas.rules" activeName="info">
					<el-tab-pane label="地区导入" name="info">
						<div class="tab-content">
							<el-form-item label="导入说明">
								<p class="grey">批量导入功能非常耗时，请谨慎使用。</p>
								<p class="grey" style="line-height:24px;">请进入 https://github.com/modood/Administrative-divisions-of-China 下载带编码的JSON数据，例如三级联动的：pca-code.json，数据格式如：{"code":"1001","name":"北京"}，将JSON格式数据复制粘贴到以下编辑框内确认导入。</p>
							</el-form-item>
							<el-form-item label="JSON数据" prop="jsonData">
								<el-input type="textarea" :rows="18" v-model="datas.importModel.jsonData" show-word-limit placeholder="JSON地区数据，不可为空"></el-input>
							</el-form-item>
						</div>
					</el-tab-pane>
				</dt-form-box>
				
				<template #footer>
					<div class="dialog-footer">
						<el-button type="primary" icon="Edit" @click="submitImport">确 定</el-button>
						<el-button type="warning" @click="datas.showImportDialog=false">取 消</el-button>
					</div>
				</template>
			</el-dialog>
			
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtToolbarBox from '../../../components/layout/DtToolbarBox.vue'
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	const treeListRef = ref(null)
	const importFormRef = ref(null)
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		showImportDialog: false, //导入窗口
		keyword: '',
		listData: [],
		importModel: {
			jsonData: ''
		},
		rules: {
			jsonData: [
				{ required: true, message: '请输入标准的JSON地区数据', trigger: 'blur' }
			]
		},
	})
	
	//加载数据
	const initData = () => {
		let sendUrl = "/admin/area"
		if (datas.keyword.length > 0) {
			sendUrl += `?keyword=${encodeURI(datas.keyword)}`
		}
		proxy.$api.request({
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
		let list = treeListRef.value.getCheckedKeys()
		//检查是否有选中
		if (list.length == 0) {
			proxy.$message({ type: 'warning', message: '请选择要删除的记录！' })
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
				url: `/admin/area?ids=${ids.toString()}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					initData() //重新加载列表
				}
			})
		}).catch(() => {})
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
				url: `/admin/area/${val}`,
				loading: true,
				successMsg: '已删除成功',
				success(res) {
					initData() //重新加载列表
				}
			});
		}).catch(() => {})
				
	}
	//修改部分字段
	const updateField = (id, path, val) => {
		proxy.$api.request({
			method: 'patch',
			url: `/admin/area/${id}`,
			data: [{ "op": "replace", "path": path, "value": val }],
			success(res) { }
		})
	}
	//确认批量导入
	const submitImport = () => {
		//调用组件验证表单
		importFormRef.value.form.validate((valid) => {
			if(valid) {
				proxy.$api.request({
					method: 'post',
					url: '/admin/area/import',
					data: datas.importModel,
					loading: true,
					successMsg: '批量导入已成功',
					success(res) {
						//初始化数据
						datas.showImportDialog = false
						datas.importModel.jsonData = ''
						initData()
					}
				})
			}
		})
	}
	
	//执行初始化方法
	initData()
</script>