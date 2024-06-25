<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统用户'},{title:'编辑角色'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="角色类型">
							<el-select v-model="datas.model.roleType" placeholder="请选择...">
								<el-option label="系统管理员" :value="1"></el-option>
								<el-option label="超级管理员" :value="2"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="name" label="角色标识">
							<el-input v-model="datas.model.name" placeholder="唯一名称，不可重复"></el-input>
						</el-form-item>
						<el-form-item prop="title" label="角色名称">
							<el-input v-model="datas.model.title" placeholder="允许中文,128字符内"></el-input>
						</el-form-item>
						<el-form-item label="分配权限" v-if="datas.model.roleType===1">
							<el-card>
								<el-tree :data="datas.model.navigation" node-key="id" default-expand-all :expand-on-click-node="false">
									<template #default="{ node, data }">
										<div class="tree-node">
											<span><el-checkbox :label="data.title" @change="(val)=>{handleNodeChange(val,node)}" /></span>
											<div class="tools-box">
												<el-checkbox v-for="item in data.resource" v-model="item.isSelected" :key="item.name">{{item.title}}</el-checkbox>
											</div>
										</div>
									</template>
								</el-tree>
							</el-card>
						</el-form-item>
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
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		model: {
			id: 0,
			name: null,
			title: null,
			roleType: null,
			isSystem: 0,
			navigation: []
		},
		rules: {
			name: [
				{ required: true, message: '请输入角色标识', trigger: 'blur' },
				{ min: 2, max: 128, message: '长度在 2 到 128 个字符', trigger: 'blur' },
				{ pattern: /^[0-9a-zA-Z_]{1,}$/, message: '只能字母数字下划线', trigger: 'blur' }
			],
			title: [
				{ required: true, message: '请输入角色标题', trigger: 'blur' },
			],
			roleType: [
				{ required: true, message: '请选择角色类型', trigger: 'change' }
			]
		}
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/manager/role/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		} else {
			await proxy.$api.request({
				url: '/admin/manager/role/menu',
				loading: true,
				success(res) {
					//赋值给model菜单
					datas.model.navigation = res.data
				}
			});
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
						url: `/admin/manager/role/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改管理角色已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/manager/role/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/manager/role',
						data: datas.model,
						successMsg: '新增管理角色已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							editFormRef.value.form.resetFields() //重置表单
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	//全选权限
	const handleNodeChange = (val, node) =>　{
		//console.log(val, node)
		if(!node.data.resource) return
		node.data.resource.forEach(item => {
			item.isSelected = val
		})
	}
	
	//运行初始化
	initData()
</script>