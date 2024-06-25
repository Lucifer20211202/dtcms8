<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员管理'},{title:'会员设置'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本设置" name="info">
					<div class="tab-content">
						<el-form-item prop="regStatus" label="注册设置">
							<el-select v-model="datas.model.regStatus" placeholder="请选择...">
								<el-option label="开放注册" :value="0"></el-option>
								<el-option label="关闭注册" :value="1"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="regVerify" label="注册审核">
							<el-radio-group v-model="datas.model.regVerify">
								<el-radio-button label="0">无须审核</el-radio-button>
								<el-radio-button label="1">人工审核</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item prop="regCtrl" label="IP注册间隔">
							<el-input v-model="datas.model.regCtrl" placeholder="必填，IP注册间隔限制0不限制(小时)">
								<template #append>小时</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="regCodeCtrl" label="验证码间隔">
							<el-input v-model="datas.model.regCodeCtrl" placeholder="必填，获取验证码间隔限制(分钟)">
								<template #append>分钟</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="regCodeLength" label="验证码长度">
							<el-input v-model="datas.model.regCodeLength" placeholder="必填，验证码生成位数">
								<template #append>位数</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="regSmsExpired" label="验证码有效期">
							<el-input v-model="datas.model.regSmsExpired" placeholder="必填，手机验证码有效期">
								<template #append>分钟</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="regEmailExpired" label="邮件有效期">
							<el-input v-model="datas.model.regEmailExpired" placeholder="必填，邮件链接有效期">
								<template #append>天</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="pointCashRate" label="积分兑换比例">
							<el-input v-model="datas.model.pointCashRate" placeholder="0为禁用兑换功能">
								<template #append>积分/1元</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="pointLoginNum" label="每天登录积分">
							<el-input v-model="datas.model.pointLoginNum" placeholder="必填，积分值为正整数">
								<template #append>积分</template>
							</el-input>
						</el-form-item>
						<el-form-item prop="regKeywords" label="用户名保留">
							<el-input v-model="datas.model.regKeywords" placeholder="必填，用户名保留关健字"></el-input>
						</el-form-item>
						<el-form-item prop="regMsgStatus" label="欢迎短信息">
							<el-select v-model="datas.model.regMsgStatus" placeholder="请选择...">
								<el-option label="不发送" :value="0"></el-option>
								<el-option label="站内消息" :value="1"></el-option>
								<el-option label="邮件消息" :value="2"></el-option>
								<el-option label="手机短信" :value="3"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="regMsgTxt" label="短信息内容">
							<el-input type="textarea" :rows="3" v-model="datas.model.regMsgTxt" placeholder="必填，500字符内"></el-input>
						</el-form-item>
						<el-form-item label="注册许可协议">
							<el-switch :active-value="1" :inactive-value="0" v-model="datas.model.regRules"></el-switch>
						</el-form-item>
						<el-form-item prop="regRulesTxt" label="许可协议内容">
							<el-input type="textarea" :rows="5" v-model="datas.model.regRulesTxt" placeholder="必填，支持HTML"></el-input>
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
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		model: {
			regStatus: 0,
			regVerify: 0,
			regMsgStatus: 0,
			regMsgTxt: null,
			regKeywords: null,
			regCtrl: 0,
			regCodeCtrl: 0,
			regCodeLength: 0,
			regSmsExpired: 0,
			regEmailExpired: 0,
			regRules: 0,
			regRulesTxt: null,
			pointCashRate: 0,
			pointLoginNum: 0
		},
		rules: {
			regStatus: [
				{ required: true, message: '注册设置不可为空', trigger: 'blur' }
			],
			regVerify: [
				{ required: true, message: '注册审核不可为空', trigger: 'blur' }
			],
			regMsgStatus: [
				{ required: true, message: '欢迎消息不可为空', trigger: 'blur' }
			],
			regMsgTxt: [{
				required: true, message: '短消息内容不可为空', trigger: 'blur'
			}],
			regKeywords: [
				{ required: true, message: '用户名保留关健字不可为空', trigger: 'blur' }
			],
			regCtrl: [
				{ required: true, message: '注册间隔限制不可为空', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能输入正整数', trigger: 'blur' }
			],
			regCodeCtrl: [
				{ required: true, message: '验证码间隔限制不可为空', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能输入正整数', trigger: 'blur' }
			],
			regCodeLength: [
				{ required: true, message: '验证码长度不可为空', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能输入正整数', trigger: 'blur' }
			],
			regSmsExpired: [
				{ required: true, message: '验证码有效期不可为空', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能输入正整数', trigger: 'blur' }
			],
			regEmailExpired: [
				{ required: true, message: '邮件有效期不可为空', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能输入正整数', trigger: 'blur' }
			],
			pointCashRate: [
				{ required: true, message: '现金积分兑换比例不可为空', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能输入正整数', trigger: 'blur' }
			],
			pointLoginNum: [
				{ required: true, message: '每天登录积分不可为空', trigger: 'blur' },
				{ pattern: /^[+]{0,1}(\d+)$/, message: '只能输入正整数', trigger: 'blur' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		await proxy.$api.request({
			url: `/admin/setting/memberconfig`,
			success(res) {
				datas.model = res.data
			}
		})
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				proxy.$api.request({
					method: 'put',
					url: `/admin/setting/memberconfig`,
					data: datas.model,
					successMsg: '会员设置已修改成功',
					beforeSend() {
						datas.loading = true
					},
					success(res) { },
					complete() {
						datas.loading = false
					}
				})
			}
		})
	}
	
	//运行初始化
	initData()
</script>