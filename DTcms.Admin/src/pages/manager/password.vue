<template>
	<div class="mainbody">
		<dt-location :data="[{title:'账户管理'},{title:'修改密码'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="修改密码" name="info">
					<div class="tab-content">
						<el-form-item prop="password" label="当前密码">
							<el-input show-password v-model="datas.model.password" placeholder="必填，最少6位英文字母"></el-input>
						</el-form-item>
						<el-form-item prop="newPassword" label="新 密 码">
							<el-input show-password v-model="datas.model.newPassword" placeholder="必填，最少6位英文字母"></el-input>
						</el-form-item>
						<el-form-item prop="confirmPassword" label="确认密码">
							<el-input show-password v-model="datas.model.confirmPassword" placeholder="必填，最少6位英文字母"></el-input>
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
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//自定义表单验证，必须放在datas之前
	const validatePass = (rule, value, callback) => {
		if (value == null || value == '') {
			callback(new Error('请输入确认密码'))
		} else if (value !== datas.model.newPassword) {
			callback(new Error("两次输入密码不一致"))
		} else {
			callback()
		}
	}
	//定义页面属性
	const datas = reactive({
		loading: false,
		model: {
			password: null,
			newPassword: null,
			confirmPassword: null
		},
		rules: {
			password: [
				{ required: true, message: '请输入当前登录密码', trigger: 'blur' },
				{ min: 6, max: 128, message: '密码长度在至少6位', trigger: 'blur' },
				{ pattern: /^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$/, message: '必须英文字母数字组合', trigger: 'blur' }
			],
			newPassword: [
				{ required: true, message: '请输入新密码', trigger: 'blur' },
				{ min: 6, max: 128, message: '密码长度在至少6位', trigger: 'blur' },
				{ pattern: /^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$/, message: '必须英文字母数字组合', trigger: 'blur' }
			],
			confirmPassword: [
				{ required: true, validator: validatePass, trigger: 'blur' }
			]
		}
	})
	
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				proxy.$api.request({
					method: 'put',
					url: '/account/manager/password',
					data: datas.model,
					successMsg: '修改密码已成功',
					beforeSend() {
						datas.loading = true
					},
					success(res) {
						editFormRef.value.form.resetFields()
					},
					complete() {
						datas.loading = false
					}
				})
			}
		})
	}
	
</script>