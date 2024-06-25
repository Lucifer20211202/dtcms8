<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统管理'},{title:'编辑频道'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item prop="siteId" label="所属站点">
							<el-select v-model="datas.model.siteId" :disabled="datas.model.id>0" placeholder="请选择...">
								<el-option v-for="item in datas.siteList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="name" label="频道标识">
							<el-input v-model="datas.model.name" :disabled="datas.model.id>0" placeholder="只允许英文字母数字"></el-input>
						</el-form-item>
						<el-form-item prop="title" label="频道标题">
							<el-input v-model="datas.model.title" placeholder="任意字符，255个字符内"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item label="是否启用">
							<el-switch :active-value="0" :inactive-value="1" v-model="datas.model.status"></el-switch>
						</el-form-item>
						<el-form-item label="允许评论">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isComment"></el-switch>
						</el-form-item>
						<el-form-item label="开启相册">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isAlbum"></el-switch>
						</el-form-item>
						<el-form-item label="开启附件">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isAttach"></el-switch>
						</el-form-item>
						<el-form-item label="允许投稿">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isContribute"></el-switch>
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="扩展字段" name="filed">
					<div class="tab-content">
						<el-row>
							<el-col :span="24">
								<el-button type="primary" icon="Plus" @click="editRow">添加</el-button>
								<el-button type="danger" icon="Delete" @click="removeRow">删除</el-button>
							</el-col>
						</el-row>
						<el-card class="table-card mat-20">
							<el-table ref="fieldTableRef" :data="datas.model.fields" class="table-form" @selection-change="handleSelectionChange">
								<el-table-column type="selection" width="55"></el-table-column>
								<el-table-column min-width="120" prop="name" label="字段名"></el-table-column>
								<el-table-column min-width="150" prop="title" label="标题"></el-table-column>
								<el-table-column min-width="100" label="控件类型">
									<template #default="scope">
										<el-tag size="small" type="success" v-if="scope.row.controlType=='input'">单行文本</el-tag>
										<el-tag size="small" type="info" v-if="scope.row.controlType=='textarea'">多行文本</el-tag>
										<el-tag size="small" type="success" v-if="scope.row.controlType=='editor'">编辑器</el-tag>
										<el-tag size="small" type="danger" v-if="scope.row.controlType=='images'">图片上传</el-tag>
										<el-tag size="small" type="success" v-if="scope.row.controlType=='video'">视频上传</el-tag>
										<el-tag size="small" type="info" v-if="scope.row.controlType=='datetime'">时间日期</el-tag>
										<el-tag size="small" type="warning" v-if="scope.row.controlType=='radio'">单选框</el-tag>
										<el-tag size="small" type="danger" v-if="scope.row.controlType=='checkbox'">复选框</el-tag>
									</template>
								</el-table-column>
								<el-table-column width="80" label="必填" align="center">
									<template #default="scope">
										<el-icon v-if="scope.row.isRequired===1"><Check /></el-icon>
										<el-icon v-else><Minus /></el-icon>
									</template>
								</el-table-column>
								<el-table-column fixed="right" width="80" label="操作" align="center">
									<template #default="scope">
										<el-link :underline="false" icon="Edit" @click="editRow(scope.$index, scope.row)"></el-link>
										<el-link :underline="false" icon="Delete" @click="deleteRow(scope.$index)"></el-link>
									</template>
								</el-table-column>
							</el-table>
						</el-card>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
		
		<el-dialog v-model="datas.fieldShowDialog" title="扩展字段" @close="resetDialog" destroy-on-close append-to-body fullscreen>
			<div class="dialog-box">
				<dt-form-box ref="fieldFormRef" v-model="datas.field" :rules="datas.rules" activeName="info">
					<el-tab-pane label="字段信息" name="info">
						<div class="tab-content">
							<el-form-item label="字段列名" prop="name" :rules="datas.rules.fields.name">
								<el-input v-model="datas.field.name" placeholder="只允许英文下划线"></el-input>
							</el-form-item>
							<el-form-item label="字段标题" prop="title" :rules="datas.rules.fields.title">
								<el-input v-model="datas.field.title" placeholder="任意字符，128个字符内"></el-input>
							</el-form-item>
							<el-form-item label="控件类型" prop="controlType" :rules="datas.rules.fields.controlType">
								<el-select v-model="datas.field.controlType" placeholder="请选择类型...">
									<el-option label="单行文本" value="input"></el-option>
									<el-option label="多行文本" value="textarea"></el-option>
									<el-option label="编辑器" value="editor"></el-option>
									<el-option label="图片上传" value="images"></el-option>
									<el-option label="视频上传" value="video"></el-option>
									<el-option label="时间日期" value="datetime"></el-option>
									<el-option label="单选框" value="radio"></el-option>
									<el-option label="复选框" value="checkbox"></el-option>
								</el-select>
							</el-form-item>
							<el-form-item v-if="datas.field.controlType=='editor'" label="编辑器类型">
								<el-radio-group v-model="datas.field.editorType">
									<el-radio-button label="0">标准型</el-radio-button>
									<el-radio-button label="1">简洁型</el-radio-button>
								</el-radio-group>
							</el-form-item>
							<el-form-item label="是否必填">
								<el-switch :active-value="1" :inactive-value="0" v-model="datas.field.isRequired"></el-switch>
							</el-form-item>
							<el-form-item v-if="datas.field.controlType=='input'" label="是否密码框">
								<el-switch :active-value="1" :inactive-value="0" v-model="datas.field.isPassword"></el-switch>
							</el-form-item>
							<el-form-item v-if="datas.field.controlType=='radio'||datas.field.controlType=='checkbox'" label="选项列表" prop="itemOption" :rules="datas.rules.fields.itemOption">
								<el-input type="textarea" v-model="datas.field.itemOption" placeholder="选项名称|值，以回车换行为一行。"></el-input>
							</el-form-item>
							<el-form-item label="默认值">
								<el-input v-model="datas.field.defaultValue" :placeholder="datas.field.controlType=='checkbox'?'初始化默认值,多个默认值用逗号分开，例如：1,2,5':'初始化默认值'"></el-input>
							</el-form-item>
							<el-form-item label="排序数字" prop="sortId" :rules="datas.rules.fields.sortId">
								<el-input v-model="datas.field.sortId" placeholder="数字越小越排前"></el-input>
							</el-form-item>
							<el-form-item label="验证提示">
								<el-input v-model="datas.field.validTipMsg" placeholder="表单验证提示信息"></el-input>
							</el-form-item>
							<el-form-item label="错误提示">
								<el-input v-model="datas.field.validErrorMsg" placeholder="表单验证错误信息"></el-input>
							</el-form-item>
							<el-form-item label="正则表达式">
								<el-input v-model="datas.field.validPattern" placeholder="表单验证正则表达式"></el-input>
							</el-form-item>
						</div>
					</el-tab-pane>
				</dt-form-box>
			</div>
			
			<template #footer>
				<div class="dialog-footer">
					<el-button type="primary" icon="Edit" @click="submitField">确 定</el-button>
					<el-button type="warning" @click="datas.fieldShowDialog = false">取 消</el-button>
				</div>
			</template>
		</el-dialog>
		
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance,nextTick } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//通知父组件响应
	const emit = defineEmits(['loadMenu'])
	//接收props传值
	const props = defineProps({
		id: 0
	})
	const editFormRef = ref(null)
	const fieldTableRef = ref(null)
	const fieldFormRef = ref(null)
	
	//定义表单属性
	const datas = reactive({
		loading: false,
		fieldDialogIndex: -1,//正在编辑的索引号
		fieldShowDialog: false,//编辑对话框
		siteList: [],//站点列表
		multipleSelection: [],
		model: {
			id: 0,
			siteId: null,
			name: '',
			title: '',
			isComment: 0,
			isAlbum: 0,
			isAttach: 0,
			isContribute: 0,
			sortId: 99,
			status: 0,
			fields: []
		},
		field: {
			id: 0,
			channelId: 0,
			name: null,
			title: null,
			controlType: null,
			itemOption: null,
			defaultValue: null,
			isPassword: 0,
			isRequired: 0,
			editorType: 0,
			validTipMsg: null,
			validErrorMsg: null,
			validPattern: null,
			sortId: 99
		},
		rules: {
			siteId: [
				{ required: true, message: '请选择所属站点', trigger: 'change' }
			],
			name: [
				{ required: true, message: '请输入频道名称', trigger: 'blur' },
				{ min: 2, max: 128, message: '长度在 2 到 128 个字符', trigger: 'blur' },
				{ pattern: /^[a-zA-Z_]{1,}$/, message: '只能字母和下划线', trigger: 'blur' }
			],
			title: [
				{ required: true, message: '请输入频道标题', trigger: 'blur' },
				{ min: 2, max: 128, message: '长度在 2 到 128 个字符', trigger: 'blur' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' }
			],
			fields: {
				name: [
					{ required: true, message: '请填写字段名称', trigger: 'blur' },
					{ min: 2, max: 128, message: '长度在 2 到 128 个字符', trigger: 'blur' },
					{ pattern: /^[a-zA-Z0-9_]{1,}$/, message: '只能字母数字和下划线', trigger: 'blur' }
				],
				title: [
					{ required: true, message: '请输入字段标题', trigger: 'blur' },
					{ min: 2, max: 128, message: '长度在 2 到 128 个字符', trigger: 'blur' }
				],
				controlType: [
					{ required: true, message: '请选择控件类型', trigger: 'change' }
				],
				itemOption: [
					{ required: true, message: '请填写选择列表', trigger: 'change' }
				],
				sortId: [
					{ required: true, message: '请输入排序数字', trigger: 'blur' }
				]
			}
		}
	})
	
	//加载数据
	const loadData = async() => {
		await proxy.$api.request({
			url: '/admin/site/view/0',
			loading: true,
			success(res) {
				datas.siteList = res.data
			}
		})
		//初始化数据
		await initData()
	}
	//初始化数据
	const initData = async() => {
		//赋值表单
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/channel/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/channel/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改频道已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							emit('loadMenu') //通知父组件重新加载菜单
							proxy.$common.linkUrl('/setting/channel/list') //跳转列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/channel',
						data: datas.model,
						successMsg: '新增频道已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
							datas.model.fields = []
							emit('loadMenu') //通知父组件重新加载菜单
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	//删除一行
	const deleteRow = (index) => {
		datas.model.fields.splice(index, 1)
	}
	//删除多行
	const removeRow = () => {
		let list = datas.multipleSelection //拿到选中的数据
		//检查是否有选中
		if (!list.length) {
			proxy.$message({ type: 'warning', message: '请选择要删除的记录' })
			return false
		}
		if (list) {
			list.forEach((item, index) => {
				//遍历源数据
				datas.model.fields.forEach((v, i) => {
					//如果选中数据和源数据的某一条唯一标识符相等，删除对应的源数据
					if (item == v) {
						datas.model.fields.splice(i, 1)
					}
				})
			})
		}
		//清除选中状态
		fieldTableRef.value.clearSelection()
	}
	//新增编辑扩展字段
	const editRow = (i, item) => {
		datas.fieldShowDialog = true
		//nextTick(() => {
			//如果有值则修改
			if (typeof i === 'number' && !isNaN(i)) {
				//赋值给model
				Object.keys(datas.field).forEach(key => { datas.field[key] = item[key] })
				datas.fieldDialogIndex = i
			}
		//})
	}
	//提交扩展字段
	const submitField = () => {
		//表单验证
		fieldFormRef.value.form.validate((valid) => {
			if (valid) {
				if (datas.fieldDialogIndex >= 0) {
					//赋值给扩展字段
					Object.keys(datas.field).forEach(key => {
						datas.model.fields[datas.fieldDialogIndex][key] = datas.field[key]
					});
				} else {
					//检查字段是否重复
					let findObj = datas.model.fields.find(item => item.name == datas.field.name)
					if(findObj) {
						proxy.$message({ type: 'warning', message: '字段名称已重复，请修改重试' })
						return false
					}
					//注意要重新创建一个对象复制，直接赋值的话拿到的是空值
					var obj = JSON.parse(JSON.stringify(datas.field))
					datas.model.fields.push(obj)
				}
				datas.fieldShowDialog = false
			}
		})
	}
	//关闭弹窗及重置字段
	const resetDialog = () => {
		datas.fieldDialogIndex = -1
		fieldFormRef.value.form.resetFields()
	}
	//扩展字段选中事件
	const handleSelectionChange = (val) => {
		datas.multipleSelection = val
	}
	
	//运行初始化
	loadData()
	
</script>