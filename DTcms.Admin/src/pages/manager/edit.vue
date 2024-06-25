<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统管理'},{title:'编辑管理员'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item label="上传头像">
							<dt-upload-image v-model="datas.model.avatar"></dt-upload-image>
						</el-form-item>
						<el-form-item label="管理角色">
							<el-select v-model="datas.model.roleId" placeholder="请选择...">
								<el-option v-for="item in datas.roleList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item label="账户状态">
							<el-radio-group v-model="datas.model.status">
								<el-radio-button label="0">正常</el-radio-button>
								<el-radio-button label="1">待验证</el-radio-button>
								<el-radio-button label="2">待审核</el-radio-button>
								<el-radio-button label="3">禁用</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="userName" label="用户名">
							<el-input v-model="datas.model.userName" placeholder="唯一名称，不可重复"></el-input>
						</el-form-item>
						<el-form-item label="登录密码" v-if="datas.model.id>0">
							<el-input show-password v-model="datas.model.password" placeholder="登录密码,不修改请留空"></el-input>
						</el-form-item>
						<el-form-item prop="password" label="登录密码" v-else>
							<el-input show-password v-model="datas.model.password" placeholder="必填，最少6位英文字母"></el-input>
						</el-form-item>
						<el-form-item prop="email" label="电子邮箱">
							<el-input v-model="datas.model.email" placeholder="非必填，电子邮箱不可重复"></el-input>
						</el-form-item>
						<el-form-item prop="phone" label="手机号码">
							<el-input v-model="datas.model.phone" placeholder="非必填，手机号不可重复"></el-input>
						</el-form-item>
						<el-form-item prop="realName" label="真实姓名">
							<el-input v-model="datas.model.realName" placeholder="非必填，可空"></el-input>
						</el-form-item>
						<el-form-item label="发布需审核">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.isAudit"></el-switch>
						</el-form-item>
						<el-form-item label="注册时间" v-if="datas.model.id>0">
							{{datas.model.addTime}}
						</el-form-item>
						<el-form-item label="最后登录时间" v-if="datas.model.lastTime">
							{{datas.model.lastTime}}
						</el-form-item>
						<el-form-item label="最后登录IP" v-if="datas.model.lastIp">
							{{datas.model.lastIp}}
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
	import DtFormBox from '../../components/layout/DtFormBox.vue'
	import DtUploadImage from '../../components/upload/DtUploadImage.vue'
	
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
		roleList: [],//角色列表
		model: {
			id: 0,
			userId: 0,
			userName: null,
			email: null,
			phone: null,
			password: null,
			roleId: null,
			status: 0,
			avatar: null,
			realName: null,
			isAudit: 0,
			addTime: null,
			lastIp: null,
			lastTime: null
		},
		rules: {
			userName: [
				{ required: true, message: '请输入登录用户名', trigger: 'blur' },
				{ min: 3, max: 128, message: '长度在 3 到 128 个字符', trigger: 'blur' }
			],
			email: [
				{ pattern: /^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$/, message: '邮箱格式有误', trigger: 'blur' }
			],
			phone: [
				{ pattern: /^1[0-9]{10}$/, message: '手机号码格式不正确', trigger: 'blur' }
			],
			password: [
				{ required: true, message: '请输入登录密码', trigger: 'blur' },
				{ min: 6, max: 128, message: '密码长度在至少6位', trigger: 'blur' },
				{ pattern: /^[a-zA-Z][a-zA-Z0-9_]*$/, message: '以字母开头至少包含数字', trigger: 'blur' }
			],
			roleId: [
				{ required: true, message: '请选择管理角色', trigger: 'change' }
			]
		}
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/manager/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
		//加载角色列表
		await proxy.$api.request({
			url: '/admin/manager/role/view/0',
			success(res) {
				datas.roleList = res.data
			}
		});
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
						url: `/admin/manager/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改管理员已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/manager/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/manager',
						data: datas.model,
						successMsg: '新增管理员已成功',
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
	
	//运行初始化
	initData()
</script>