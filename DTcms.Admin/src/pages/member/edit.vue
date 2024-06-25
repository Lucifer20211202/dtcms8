<template>
	<div class="mainbody">
		<dt-location :data="[{title:'会员管理'},{title:'编辑会员'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="account">
				<el-tab-pane label="账户信息" name="account">
					<div class="tab-content">
						<el-form-item label="所属站点">
							<el-select v-model="datas.model.siteId" placeholder="请选择...">
								<el-option v-for="item in datas.siteList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item label="所属组别">
							<el-select v-model="datas.model.groupId" placeholder="请选择...">
								<el-option v-for="item in datas.groupList"
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
								<el-radio-button label="3">黑名单</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item label="推荐会员">
							<dt-member-select v-model="datas.model.referrerId"></dt-member-select>
						</el-form-item>
						<el-form-item prop="userName" label="用 户 名">
							<el-input v-model="datas.model.userName" placeholder="唯一名称，不可重复"></el-input>
						</el-form-item>
						<el-form-item label="登录密码" v-if="datas.model.id>0">
							<el-input v-model="datas.model.password" placeholder="登录密码,不修改请留空"></el-input>
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
						<el-form-item label="累计消费" v-if="datas.model.id>0">
							{{datas.model.orderAmount}} 元
						</el-form-item>
						<el-form-item label="累计佣金" v-if="datas.model.id>0&&datas.model.isReseller==1">
							{{datas.model.commAmount}} 元
						</el-form-item>
						<el-form-item label="账户余额" v-if="datas.model.id>0">
							{{datas.model.amount}} 元
						</el-form-item>
						<el-form-item label="账户积分" v-if="datas.model.id>0">
							{{datas.model.point}} 分
						</el-form-item>
						<el-form-item label="经 验 值" v-if="datas.model.id>0">
							{{datas.model.exp}}
						</el-form-item>
						<el-form-item label="到期时间" v-if="datas.model.expiryTime">
							{{datas.model.expiryTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
				<el-tab-pane label="个人资料" name="info">
					<div class="tab-content">
						<el-form-item label="会员头像">
							<dt-upload-image v-model="datas.model.avatar"></dt-upload-image>
						</el-form-item>
						<el-form-item label="真实姓名">
							<el-input v-model="datas.model.realName" placeholder="非必填，可空"></el-input>
						</el-form-item>
						<el-form-item prop="sex" label="性别">
							<el-radio-group v-model="datas.model.sex">
								<el-radio-button label="保密">保密</el-radio-button>
								<el-radio-button label="男">男</el-radio-button>
								<el-radio-button label="女">女</el-radio-button>
							</el-radio-group>
						</el-form-item>
						<el-form-item label="生日">
							<el-date-picker v-model="datas.model.birthday" type="date" value-format="yyyy-MM-dd" placeholder="选择日期"></el-date-picker>
						</el-form-item>
						<el-form-item label="注册时间" v-if="datas.model.id>0">
							{{datas.model.regTime}}
						</el-form-item>
						<el-form-item label="注册IP" v-if="datas.model.id>0">
							{{datas.model.regIp}}
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
	import DtMemberSelect from '../../components/control/DtMemberSelect.vue'
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
		siteList: [],//站点列表
		groupList: [], //会员组列表
		model: {
			id: 0,
			userId: null,
			referrerId: 0,
			siteId: null,
			groupId: null,
			groupTitle: null,
			userName: null,
			email: null,
			phone: null,
			password: null,
			avatar: null,
			realName: null,
			sex: '保密',
			birthday: null,
			amount: 0,
			point: 0,
			exp: 0,
			status: 0,
			regIp: null,
			regTime: null,
			lastIp: null,
			lastTime: null,
		},
		rules: {
			siteId: [
				{ required: true, message: '请选择所属站点', trigger: 'change' }
			],
			groupId: [
				{ required: true, message: '请选择所属会员组', trigger: 'change' }
			],
			userName: [
				{ required: true, message: '请输入登录用户名', trigger: 'blur' },
				{ min: 3, max: 128, message: '长度在 3 到 128 个字符', trigger: 'blur' },
				/*{ pattern: /^[a-zA-Z0-9_]*$/, message: '只能是字母数字下划线', trigger: 'blur' }*/
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
			sex: [
				{ required: true, message: '请选择性别', trigger: 'change' }
			]
		},
	})
	
	//初始化数据
	const initData = async() => {
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/member/${props.id}`,
				loading: true,
				success(res) {
					datas.model = res.data
				}
			})
		}
		//加载站点列表
		await proxy.$api.request({
			url: '/admin/site/view/0',
			success(res) {
				datas.siteList = res.data
			}
		});
		//加载会员组列表
		await proxy.$api.request({
			url: '/admin/member/group/view/0',
			success(res) {
				datas.groupList = res.data
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
						url: `/admin/member/${datas.model.userId}`,
						data: datas.model,
						successMsg: '修改会员已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							proxy.$common.linkUrl('/member/list') //跳转加列表页
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/member',
						data: datas.model,
						successMsg: '新增会员已成功',
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